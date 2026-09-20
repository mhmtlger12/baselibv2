using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class JobsController(ISiteContentService site) : Controller
{
    [HttpGet("/ilanlar")] public IActionResult Index(string? category, string? q) => View(site.Jobs(category, q));
    [HttpGet("/ilanlar/{id:int}")]
    public IActionResult Detail(int id)
    {
        var job = site.Content.JobListings.Find(x => x.Id == id);
        return job is null ? NotFound() : View(job);
    }
}
