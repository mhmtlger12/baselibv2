using BaseLib.Presentation.Services.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;

[Authorize(Roles = "Admin"), Route("Admin/Content/{slug}")]
public sealed class ContentController(IContentService content) : AdminController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string slug, CancellationToken ct)
    {
        if (ExistingController(slug) is string controller) return RedirectToAction("Index", controller);
        var page = await content.ListAsync(slug, ct);
        return page is null ? NotFound() : View(page);
    }
    [HttpGet("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(string slug, CancellationToken ct, int id = 0)
    {
        if (ExistingController(slug) is string controller) return RedirectToAction("Index", controller);
        var model = await content.EditAsync(slug, id, ct);
        return model is null ? NotFound() : View(model);
    }
    [HttpPost("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(string slug, int id, ContentEditModel model, CancellationToken ct)
    {
        if (ExistingController(slug) is not null) return StatusCode(StatusCodes.Status410Gone);
        var existing = await content.EditAsync(slug, id, ct);
        if (existing is null) return NotFound();
        model.Slug = slug; model.Id = id; model.Definition = existing.Definition;
        if (!ModelState.IsValid || !await ExecuteAsync(() => content.SaveAsync(model, ct))) return View(model);
        TempData["Success"] = "İçerik kaydedildi.";
        return RedirectToAction(nameof(Index), new { slug });
    }
    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(string slug, int id, CancellationToken ct)
    {
        if (ExistingController(slug) is string controller) return RedirectToAction("Index", controller);
        var model = await content.EditAsync(slug, id, ct);
        return model is null ? NotFound() : View(model);
    }
    [HttpPost("Delete/{id:int}"), ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(string slug, int id, CancellationToken ct)
    {
        if (ExistingController(slug) is not null) return StatusCode(StatusCodes.Status410Gone);
        if (await ExecuteAsync(() => content.DeleteAsync(slug, id, ct)))
        { TempData["Success"] = "İçerik çöp kutusuna taşındı."; return RedirectToAction(nameof(Index), new { slug }); }
        var model = await content.EditAsync(slug, id, ct);
        return model is null ? NotFound() : View("Delete", model);
    }
    private static string? ExistingController(string slug) => slug.ToLowerInvariant() switch
    { "jobs" => "Jobs", "slider" => "Sliders", "institutions" => "Institutions", _ => null };
}
