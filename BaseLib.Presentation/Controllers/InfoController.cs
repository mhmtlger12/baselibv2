using BaseLib.Presentation.Models.Site;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;

public sealed class InfoController(ISiteContentService site) : Controller
{
    [HttpGet("/bilgi/{slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken ct)
    {
        var page = (await site.GetAsync(ct)).Pages.FirstOrDefault(x => x.Slug == slug);
        if (page is null) return NotFound();
        ViewData["Title"] = page.Title;
        return View(page);
    }
    [HttpGet("/iletisim")] public IActionResult Contact() => View(new ContactInput());
    [HttpPost("/iletisim")]
    public async Task<IActionResult> Contact(ContactInput input, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(input);
        try { await site.SendContactAsync(input, ct); }
        catch (ApiException error) when (error.StatusCode is 400 or 429)
        {
            ModelState.AddModelError("", error.StatusCode == 429 ? "Kısa sürede çok fazla mesaj gönderdiniz. Lütfen biraz bekleyin." : error.Message);
            return View(input);
        }
        TempData["Success"] = "Mesajınız alındı.";
        return RedirectToAction(nameof(Contact));
    }
}
