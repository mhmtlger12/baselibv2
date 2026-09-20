using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class HomeController(ISiteContentService site) : Controller
{
    [HttpGet("/")] public IActionResult Index() => View(site.Content);
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
