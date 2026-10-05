using Baselib.Business.DTOs;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Services.Site;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Controllers;

public sealed class ScoresController(ISiteContentService site) : Controller
{
    [HttpGet("/taban-puanlari/{category}")]
    public async Task<IActionResult> Index(string category, string? level, string? q, CancellationToken ct)
    {
        var page = await site.DepartmentsAsync(category, level, q, ct);
        return page is null ? NotFound() : View(page);
    }
    [HttpGet("/taban-puanlari/{category}/bolum/{id:int}")]
    public async Task<IActionResult> Detail(string category, int id, string? period, string? institution, string? city, CancellationToken ct, int page = 1)
    {
        var model = await site.DetailAsync(category, id, period, institution, city, page, ct);
        return model is null ? NotFound() : View(model);
    }
    [HttpPost("/taban-puanlari/{category}/bolum/{id:int}/yorum")]
    public async Task<IActionResult> Comment(string category, int id, SendCommentDto input, CancellationToken ct)
    {
        input.Page = $"/taban-puanlari/{category}/bolum/{id}";
        ModelState.Remove(nameof(input.Page));
        if (ModelState.IsValid)
        {
            try
            {
                await site.SendCommentAsync(input, ct);
                TempData["CommentSuccess"] = "Yorumunuz alındı. Onaylandıktan sonra yayımlanacak.";
                return RedirectToAction(nameof(Detail), new { category, id });
            }
            catch (ApiException error) when (error.StatusCode is 400 or 429)
            { ModelState.AddModelError("", error.StatusCode == 429 ? "Kısa sürede çok fazla yorum gönderdiniz. Lütfen biraz bekleyin." : error.Message); }
        }
        var model = await site.DetailAsync(category, id, null, null, null, 1, ct);
        ViewData["CommentDraft"] = input;
        return model is null ? NotFound() : View("Detail", model);
    }
}
