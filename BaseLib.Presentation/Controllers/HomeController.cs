using BaseLib.Presentation.Services.Site;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Models.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class HomeController(ISiteContentService site, IPublicJobApiService jobs) : Controller
{
    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var listings = await jobs.GetPublishedAsync("all", null, ct);
        var data = await site.GetAsync(ct);
        return View(new HomePage(data.Countdowns, data.ScoreCards,
            listings.Select(JobListing.FromDto).ToArray()));
    }
    [HttpGet("/arama")]
    public async Task<IActionResult> Search(string? q, CancellationToken ct)
    {
        var listings = await jobs.GetPublishedAsync("all", null, ct);
        return View(await site.SearchAsync(q, listings.Select(JobListing.FromDto).ToArray(), ct));
    }
    [Route("/hata/{code:int?}")]
    public IActionResult Error(int code = 500)
    {
        Response.StatusCode = code;
        ViewData["Title"] = code == 404 ? "Sayfa bulunamadı" : "İşlem tamamlanamadı";
        ViewData["Message"] = code == 404 ? "Aradığınız sayfa bulunamadı." : "İsteğiniz tamamlanamadı. Lütfen tekrar deneyin.";
        return View("~/Views/Shared/Error.cshtml");
    }
}
