namespace Baselib.Business.DTOs;

public sealed class SiteContentDto
{
    public List<SiteNavDto> NavLinks { get; set; } = [];
    public List<SiteCountdownDto> Countdowns { get; set; } = [];
    public List<SiteScoreCardDto> ScoreCards { get; set; } = [];
    public List<SiteNewsDto> RecentItems { get; set; } = [];
    public List<SiteNewsDto> Announcements { get; set; } = [];
    public List<SiteLevelDto> LevelTabs { get; set; } = [];
    public List<SiteProgramDto> Departments { get; set; } = [];
    public List<SitePeriodDto> Periods { get; set; } = [];
    public List<SiteAdDto> Ads { get; set; } = [];
    public List<SitePageDto> Pages { get; set; } = [];
}
public sealed record SiteNavDto(string Label, string Url);
public sealed record SiteCountdownDto(string Key, string Label, DateTime Target);
public sealed record SiteScoreCardDto(int Id, string Key, string Title, string Category, string Href);
public sealed record SiteNewsDto(int Id, string Title, string Date, string Link);
public sealed record SiteLevelDto(int Id, string Key, string Label);
public sealed record SiteProgramDto(int Id, string Name, string Level);
public sealed record SitePeriodDto(int Id, string Name);
public sealed record SiteAdDto(string Name, string Position, string ImageUrl, string LinkUrl);
public sealed record SitePageDto(string Slug, string Title, string Body);
public sealed record SiteScoreRowDto(int Id, string Institution, string City, string Title, int Quota, int Vacant, decimal MinScore, decimal MaxScore, string Qualification, int Rank);
public sealed record PublicScoresDto(string Period, int Page, int PageCount, int TotalCount, int TotalQuota, decimal? MinScore, decimal? MaxScore, IReadOnlyList<SiteScoreRowDto> Rows);
public sealed record SiteCommentDto(int Id, string Author, string Time, string Body, int Likes, List<SiteCommentDto> Replies);
