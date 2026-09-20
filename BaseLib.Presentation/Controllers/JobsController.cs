using BaseLib.Presentation.Services.Site;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Models.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class JobsController(ISiteContentService site, IPublicJobApiService jobs) : Controller
{
    [HttpGet("/ilanlar")]
    public async Task<IActionResult> Index(string? category, string? q, CancellationToken ct)
    {
        var node = site.Content.JobCategories.SelectMany(x => new[] { x }.Concat(x.Children ?? [])).FirstOrDefault(x => x.Key == category);
        var items = await jobs.GetPublishedAsync(category, q, ct);
        var model = new JobsPage(category ?? "all", q ?? "", node?.Label ?? "Tüm İlanlar", items.Select(ToModel).ToList());
        return View(model);
    }

    [HttpGet("/ilanlar/{id:int}")]
    public async Task<IActionResult> Detail(int id, CancellationToken ct)
    {
        var dto = await jobs.GetAsync(id, ct);
        var job = dto is null ? null : ToModel(dto);
        return job is null ? NotFound() : View(job);
    }

    private static JobListing ToModel(Baselib.Business.DTOs.JobListingDto dto) =>
        new(dto.Id, dto.Institution, dto.Summary, dto.CategoryKey, dto.CategoryLabel, dto.PublishedAt,
            dto.StartDate, dto.EndDate, dto.SourceUrl, dto.PdfUrl, dto.InstitutionLogoUrl);
}
