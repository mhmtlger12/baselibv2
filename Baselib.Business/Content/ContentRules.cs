using System.Globalization;
using Baselib.Business.DTOs;
using Baselib.Core.Interfaces;
using Baselib.Entities;

namespace Baselib.Business.Content;

public sealed class ContentRules(
    IEntityRepository<ScoreCategory> categories, IEntityRepository<StudyProgram> programs,
    IEntityRepository<StudyLevel> levels, IEntityRepository<ScorePeriod> periods,
    IEntityRepository<JobCategory> jobCategories, IEntityRepository<InformationPage> pages,
    IEntityRepository<PublicComment> comments)
{
    private static ContentOptionDto Option(int id, string label) => new(id.ToString(CultureInfo.InvariantCulture), label);
    public async Task<IReadOnlyList<ContentOptionDto>> OptionsAsync(string lookup, CancellationToken ct) => lookup switch
    {
        "positions" => [new("home", "Anasayfa Üst"), new("sidebar", "Sidebar")],
        "news-categories" => [new("KPSS", "KPSS"), new("YKS", "YKS"), new("ALES", "ALES"), new("DGS", "DGS")],
        "comment-status" => [new("Onaylı", "Onaylı"), new("Beklemede", "Beklemede"), new("Spam", "Spam")],
        "message-status" => [new("Yeni", "Yeni"), new("Okundu", "Okundu")],
        "levels" => (await levels.GetAllAsync(asNoTracking: true, cancellationToken: ct)).Select(x => Option(x.Id, x.Label)).ToList(),
        "programs" => (await programs.GetAllAsync(x => x.StudyLevel!.IsActive && !x.StudyLevel.IsDeleted, asNoTracking: true, cancellationToken: ct)).Select(x => Option(x.Id, x.Name)).ToList(),
        "periods" => (await periods.GetAllAsync(asNoTracking: true, cancellationToken: ct)).OrderBy(x => x.SortOrder).Select(x => Option(x.Id, x.Name)).ToList(),
        "score-kpss" or "score-dgs" or "score-yks" => (await categories.GetAllAsync(asNoTracking: true, cancellationToken: ct))
            .Where(x => lookup == "score-kpss" ? x.Key.StartsWith("kpss", StringComparison.Ordinal) : x.Key == lookup[6..]).Select(x => Option(x.Id, x.Title)).ToList(),
        "job-parents" => new[] { new ContentOptionDto("", "Üst kategori yok") }.Concat((await jobCategories.GetAllAsync(asNoTracking: true, cancellationToken: ct))
            .Where(x => !x.IsSelectable).Select(x => new ContentOptionDto(x.Key, x.Label))).ToList(),
        _ => []
    };

    public async Task<string?> ValidateAsync(SoftDeleteEntity item, CancellationToken ct)
    {
        switch (item)
        {
            case ScoreCategory category:
                if (await categories.AnyAsync(x => x.Id != category.Id && x.Key == category.Key, ignoreQueryFilters: true, cancellationToken: ct)) return "Kategori anahtarı zaten kullanılıyor.";
                break;
            case StudyLevel level:
                if (await levels.AnyAsync(x => x.Id != level.Id && x.Key == level.Key, ignoreQueryFilters: true, cancellationToken: ct)) return "Öğrenim düzeyi anahtarı zaten kullanılıyor.";
                break;
            case ScorePeriod period:
                if (await periods.AnyAsync(x => x.Id != period.Id && x.Name == period.Name, ignoreQueryFilters: true, cancellationToken: ct)) return "Dönem zaten kayıtlı.";
                break;
            case InformationPage page:
                if (await pages.AnyAsync(x => x.Id != page.Id && x.Slug == page.Slug, ignoreQueryFilters: true, cancellationToken: ct)) return "Sayfa adresi zaten kullanılıyor.";
                break;
            case ScoreEntry score:
                if (score.MinScore > score.MaxScore) return "En düşük puan en yüksek puanı aşamaz.";
                if (score.Vacant > score.Quota) return "Boş kontenjan toplam kontenjanı aşamaz.";
                break;
            case ExamCountdown countdown when countdown.Target == default:
                return "Sınav tarihi zorunludur.";
            case SiteNews news when news.PublishedAt == default:
                return "Yayın tarihi zorunludur.";
            case PublicComment comment:
                if (!await IsScorePageAsync(comment.Page, ct)) return "Yorum için geçerli bir puan detay sayfası seçin.";
                if (comment.ParentId is int parentId && !await comments.AnyAsync(x => x.Id == parentId && x.Page == comment.Page && x.Status == "Onaylı", cancellationToken: ct)) return "Yanıt verilen yorum bu sayfada bulunamadı.";
                break;
            case JobCategory category:
                category.ParentKey = string.IsNullOrWhiteSpace(category.ParentKey) ? null : category.ParentKey;
                if (category.Key == "all" || !System.Text.RegularExpressions.Regex.IsMatch(category.Key, "^[a-z0-9]+(?:-[a-z0-9]+)*$")) return "Geçerli bir kategori anahtarı girin.";
                if (await jobCategories.AnyAsync(x => x.Id != category.Id && x.Key == category.Key, ignoreQueryFilters: true, cancellationToken: ct)) return "Kategori anahtarı zaten kullanılıyor.";
                // The current public navigation supports groups and one level of selectable children.
                if (category.ParentKey is not null)
                {
                    var parent = await jobCategories.FirstOrDefaultAsync(x => x.Key == category.ParentKey, cancellationToken: ct);
                    if (parent is null || parent.Id == category.Id || parent.ParentKey is not null || parent.IsSelectable || !category.IsSelectable)
                        return "Alt kategori için üst düzey bir kategori grubu seçin.";
                }
                if ((category.IsSelectable || category.ParentKey is not null) && await jobCategories.AnyAsync(x => x.ParentKey == category.Key && !x.IsDeleted, ignoreQueryFilters: true, cancellationToken: ct)) return "Alt kategorisi olan kayıt grup olarak kalmalıdır.";
                break;
        }
        return null;
    }

    public async Task<bool> IsScorePageAsync(string page, CancellationToken ct)
    {
        var parts = page.Split('/');
        return parts.Length == 5 && parts[0] == "" && parts[1] == "taban-puanlari" && parts[3] == "bolum" &&
            int.TryParse(parts[4], out var id) && await categories.AnyAsync(x => x.Key == parts[2], cancellationToken: ct) &&
            await programs.AnyAsync(x => x.Id == id && x.StudyLevel!.IsActive && !x.StudyLevel.IsDeleted, cancellationToken: ct);
    }
}
