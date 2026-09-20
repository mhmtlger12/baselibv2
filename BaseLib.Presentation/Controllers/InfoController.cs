using BaseLib.Presentation.Models.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;
public sealed class InfoController : Controller
{
    private static readonly Dictionary<string, string> Pages = new()
    { ["hakkimizda"] = "Hakkımızda", ["gizlilik"] = "Gizlilik Politikası", ["kullanim-kosullari"] = "Kullanım Koşulları", ["ales"] = "ALES" };
    [HttpGet("/bilgi/{slug}")]
    public IActionResult Index(string slug)
    {
        if (!Pages.TryGetValue(slug, out var title)) return NotFound();
        ViewData["Title"] = title;
        return View();
    }
    [HttpGet("/iletisim")] public IActionResult Contact() => View(new ContactInput());
    [HttpPost("/iletisim")]
    public IActionResult Contact(ContactInput input)
    {
        if (!ModelState.IsValid) return View(input);
        TempData["Success"] = "Örnek mesajınız alındı. Bu form şu anda demo olarak çalışıyor.";
        return RedirectToAction(nameof(Contact));
    }
}
