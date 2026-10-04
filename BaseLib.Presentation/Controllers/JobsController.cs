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
        // The visible list and category counts use the same response for this request.
        var items = await jobs.GetPublishedAsync("all", null, ct);
        return View(site.Jobs(category, q, items.Select(JobListing.FromDto).ToArray()));
    }

    [HttpGet("/ilanlar/{id:int}")]
    public async Task<IActionResult> Detail(int id, CancellationToken ct)
    {
        var dto = await jobs.GetAsync(id, ct);
        var job = dto is null ? null : JobListing.FromDto(dto);
        return job is null ? NotFound() : View(job);
    }
}
