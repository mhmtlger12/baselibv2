using BaseLib.Presentation.Services.Site;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class HomeController(ISiteContentService site, IPublicJobApiService jobs) : Controller
{
    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var listings = await jobs.GetPublishedAsync("all", null, ct);
        site.Content.JobListings.Clear();
        site.Content.JobListings.AddRange(listings.Select(item => new BaseLib.Presentation.Models.Site.JobListing(
            item.Id, item.Institution, item.Summary, item.CategoryKey, item.CategoryLabel,
            item.PublishedAt, item.StartDate, item.EndDate, item.SourceUrl, item.PdfUrl)));
        return View(site.Content);
    }
    [HttpGet("/arama")] public IActionResult Search(string? q) => View(site.Search(q));
    [Route("/hata/{code:int?}")]
    public IActionResult Error(int code = 500)
    {
        Response.StatusCode = code;
        ViewData["Title"] = code == 404 ? "Sayfa bulunamadı" : "İşlem tamamlanamadı";
        ViewData["Message"] = code == 404 ? "Aradığınız sayfa bulunamadı." : "İsteğiniz tamamlanamadı. Lütfen tekrar deneyin.";
        return View("~/Views/Shared/Error.cshtml");
    }
}
