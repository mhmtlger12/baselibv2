namespace Baselib.Entities;

public sealed class Institution : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<JobListing> JobListings { get; set; } = [];
}
