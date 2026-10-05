using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Baselib.Business.Content;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class SiteContentService(
    IEntityRepository<SiteNavigation> navigation, IEntityRepository<ExamCountdown> countdowns,
    IEntityRepository<SiteAdvertisement> advertisements, IEntityRepository<SiteNews> news,
    IEntityRepository<ScoreCategory> categories, IEntityRepository<StudyLevel> levels,
    IEntityRepository<StudyProgram> programs, IEntityRepository<ScorePeriod> periods,
    IEntityRepository<ScoreEntry> scores, IEntityRepository<PublicComment> comments,
    IEntityRepository<ContactMessage> messages, IEntityRepository<InformationPage> pages,
    ContentRules rules, IUnitOfWork unitOfWork, TimeProvider clock) : ISiteContentService
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public async Task<IDataResult<SiteContentDto>> GetAsync(CancellationToken ct)
    {
        // Repositories share a scoped DbContext; queries are intentionally sequential.
        var result = new SiteContentDto
        {
            NavLinks = (await navigation.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Select(x => new SiteNavDto(x.Label, x.Url)).ToList(),
            Countdowns = (await countdowns.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Select(x => new SiteCountdownDto(x.Id.ToString(CultureInfo.InvariantCulture), x.Label, x.Target)).ToList(),
            ScoreCards = (await categories.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Select(x => new SiteScoreCardDto(x.Id, x.Key, x.Title, x.Label, x.Key)).ToList(),
            LevelTabs = (await levels.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Select(x => new SiteLevelDto(x.Id, x.Key, x.Label)).ToList(),
            Departments = (await programs.GetAllAsync(x => x.StudyLevel!.IsActive && !x.StudyLevel.IsDeleted, asNoTracking: true, cancellationToken: ct, includes: [x => x.StudyLevel!])).OrderBy(x => x.Name).Select(x => new SiteProgramDto(x.Id, x.Name, x.StudyLevel!.Key)).ToList(),
            Periods = (await periods.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).Select(x => new SitePeriodDto(x.Id, x.Name)).ToList(),
            Ads = (await advertisements.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.Id).Select(x => new SiteAdDto(x.Name, x.Position, x.ImageUrl, x.LinkUrl)).ToList(),
            Pages = (await pages.GetAllAsync(asNoTracking: true, cancellationToken: ct)).Select(x => new SitePageDto(x.Slug, x.Title, x.Body)).ToList()
        };
        var now = clock.GetUtcNow().UtcDateTime;
        var items = (await news.GetAllAsync(x => x.PublishedAt <= now, asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenByDescending(x => x.PublishedAt).ThenBy(x => x.Id).ToList();
        result.RecentItems = items.Where(x => x.Section == "recent").Select(News).ToList();
        result.Announcements = items.Where(x => x.Section == "osym").Select(News).ToList();
        return DataResult<SiteContentDto>.Ok(result);
    }

    public async Task<IDataResult<PublicScoresDto>> ScoresAsync(string category, int programId, string? period, string? institution, string? city, int page, CancellationToken ct)
    {
        if (!await rules.IsScorePageAsync($"/taban-puanlari/{category}/bolum/{programId}", ct)) return DataResult<PublicScoresDto>.NotFound();
        var availablePeriods = (await periods.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToList();
        var periodsWithScores = await scores.SelectAsync(query => query.Where(x => x.ScoreCategory!.Key == category &&
            x.StudyProgramId == programId && x.ScorePeriod!.IsActive && !x.ScorePeriod.IsDeleted).Select(x => x.ScorePeriodId).Distinct(), ct);
        var selectedPeriod = availablePeriods.FirstOrDefault(x => x.Name == period)
            ?? availablePeriods.FirstOrDefault(x => periodsWithScores.Contains(x.Id)) ?? availablePeriods.FirstOrDefault();
        var periodId = selectedPeriod?.Id ?? 0;
        institution = institution?.Trim(); city = city?.Trim();
        var rows = (await scores.GetAllAsync(x => x.ScoreCategory!.Key == category && x.ScoreCategory.IsActive && !x.ScoreCategory.IsDeleted &&
            x.StudyProgramId == programId && x.StudyProgram!.IsActive && !x.StudyProgram.IsDeleted &&
            x.StudyProgram.StudyLevel!.IsActive && !x.StudyProgram.StudyLevel.IsDeleted &&
            x.ScorePeriodId == periodId && x.ScorePeriod!.IsActive && !x.ScorePeriod.IsDeleted &&
            (string.IsNullOrEmpty(institution) || x.Institution.Contains(institution)) &&
            (string.IsNullOrEmpty(city) || x.City.Contains(city)), asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.Id).ToList();
        var pageCount = Math.Max(1, (int)Math.Ceiling(rows.Count / 5d));
        page = Math.Clamp(page, 1, pageCount);
        return DataResult<PublicScoresDto>.Ok(new(selectedPeriod?.Name ?? "", page, pageCount, rows.Count,
            rows.Sum(x => x.Quota), rows.Count == 0 ? null : rows.Min(x => x.MinScore), rows.Count == 0 ? null : rows.Max(x => x.MaxScore),
            rows.Skip((page - 1) * 5).Take(5).Select(x => new SiteScoreRowDto(x.Id, x.Institution, x.City, x.Title, x.Quota, x.Vacant, x.MinScore, x.MaxScore, x.Qualification, x.Rank)).ToList()));
    }

    public async Task<IDataResult<IReadOnlyList<SiteCommentDto>>> CommentsAsync(string page, CancellationToken ct)
    {
        if (!await rules.IsScorePageAsync(page, ct)) return DataResult<IReadOnlyList<SiteCommentDto>>.NotFound();
        var rows = (await comments.GetAllAsync(x => x.Page == page && x.Status == "Onaylı", asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.CreatedDate).ThenBy(x => x.Id).ToList();
        var visited = new HashSet<int>();
        List<SiteCommentDto> Build(int? parentId) => rows.Where(x => x.ParentId == parentId && visited.Add(x.Id)).Select(x =>
            new SiteCommentDto(x.Id, x.HideName ? string.Join(" ", x.Author.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(part => part[0] + "***")) : x.Author,
                x.CreatedDate.ToString("d MMMM yyyy", Turkish), x.Body, x.Likes, Build(x.Id))).ToList();
        return DataResult<IReadOnlyList<SiteCommentDto>>.Ok(Build(null));
    }

    public async Task<IResult> SendContactAsync(SendContactDto input, CancellationToken ct)
    {
        var error = Validate(input);
        if (error is not null) return Result.BadRequest(error);
        await messages.AddAsync(new ContactMessage { Name = input.Name.Trim(), Email = input.Email.Trim(), Subject = input.Subject.Trim(), Message = input.Message.Trim(), Status = "Yeni", CreatedDate = clock.GetUtcNow().UtcDateTime }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("Mesajınız alındı.");
    }

    public async Task<IResult> SendCommentAsync(SendCommentDto input, CancellationToken ct)
    {
        var error = Validate(input);
        if (error is not null) return Result.BadRequest(error);
        if (!await rules.IsScorePageAsync(input.Page, ct)) return Result.BadRequest("Geçerli bir puan sayfası seçin.");
        if (input.ParentId is int parentId && !await comments.AnyAsync(x => x.Id == parentId && x.Page == input.Page && x.Status == "Onaylı", cancellationToken: ct))
            return Result.BadRequest("Yanıt verilen yorum bu sayfada bulunamadı.");
        await comments.AddAsync(new PublicComment { Author = input.Author.Trim(), Email = input.Email.Trim(), Body = input.Body.Trim(), Page = input.Page,
            ParentId = input.ParentId, HideName = input.HideName, Status = "Beklemede", CreatedDate = clock.GetUtcNow().UtcDateTime }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok("Yorumunuz alındı. Onaylandıktan sonra yayımlanacak.");
    }

    private static SiteNewsDto News(SiteNews item) => new(item.Id, item.Title, item.PublishedAt.ToString("d MMMM yyyy", Turkish), item.Link);
    private static string? Validate(object value)
    {
        var errors = new List<ValidationResult>();
        return Validator.TryValidateObject(value, new(value), errors, true) ? null : string.Join(" ", errors.Select(x => x.ErrorMessage));
    }
}
