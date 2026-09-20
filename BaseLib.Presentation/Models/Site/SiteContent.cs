namespace BaseLib.Presentation.Models.Site;
public sealed class SiteContent
{
    public List<NavLink> NavLinks { get; init; } = [];
    public List<Countdown> Countdowns { get; init; } = [];
    public List<ScoreCard> ScoreCards { get; init; } = [];
    public List<NewsItem> RecentItems { get; init; } = [];
    public List<NewsItem> Announcements { get; init; } = [];
    public List<LevelTab> LevelTabs { get; init; } = [];
    public List<StudyDepartment> Departments { get; init; } = [];
    public List<string> DetailPeriods { get; init; } = [];
    public List<ScoreRow> ScoreRows { get; init; } = [];
    public List<JobCategory> JobCategories { get; init; } = [];
    public List<JobListing> JobListings { get; init; } = [];
    public List<SiteComment> Comments { get; init; } = [];
}
public sealed record NavLink(string Key, string Label);
public sealed record Countdown(string Key, string Label, DateTime Target);
public sealed record ScoreCard(string Key, string Title, string Category, string Href);
public sealed record NewsItem(int Id, string Title, string Date);
public sealed record LevelTab(string Key, string Label);
public sealed record StudyDepartment(int Id, string Name, string Level);
public sealed record ScoreRow(int Id, string Institution, string City, string Title, int Quota, int Vacant, decimal MinScore, decimal MaxScore, string Qualification);
public sealed record JobCategory(string Key, string Label, List<JobCategory>? Children);
public sealed record JobListing(int Id, string Institution, string Summary, string CategoryKey, string CategoryLabel, DateTime PublishedAt, string StartDate, string EndDate);
public sealed record SiteComment(int Id, string Author, string Time, string Body, int Likes, List<SiteComment> Replies);
