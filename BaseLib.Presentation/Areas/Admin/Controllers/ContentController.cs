using BaseLib.Presentation.Services.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
[Authorize(Roles = "Admin")]
[Route("Admin/Content/{slug}")]
public sealed class ContentController(IContentService content) : AdminController
{
    [HttpGet("")]
    public IActionResult Index(string slug)
    {
        var page = content.List(slug);
        return page is null ? NotFound() : View(page);
    }
    [HttpGet("Edit/{id:int?}")]
    public IActionResult Edit(string slug, int id = 0)
    {
        var model = content.Edit(slug, id);
        return model is null ? NotFound() : View(model);
    }
    [HttpPost("Edit/{id:int?}")]
    public IActionResult Edit(string slug, int id, ContentEditModel model)
    {
        var existing = content.Edit(slug, id);
        if (existing is null) return NotFound();
        model.Slug = slug; model.Id = id; model.Definition = existing.Definition;
        if (!ModelState.IsValid) return View(model);
        foreach (var error in content.Save(model)) ModelState.AddModelError(error.Key, error.Value);
        if (!ModelState.IsValid) return View(model);
        TempData["Success"] = "Kayıt kaydedildi.";
        return RedirectToAction(nameof(Index), new { slug });
    }
    [HttpGet("Delete/{id:int}")]
    public IActionResult Delete(string slug, int id)
    {
        var model = content.Edit(slug, id);
        return model is null ? NotFound() : View(model);
    }
    [HttpPost("Delete/{id:int}"), ActionName("Delete")]
    public IActionResult ConfirmDelete(string slug, int id)
    {
        if (!content.Delete(slug, id)) return NotFound();
        TempData["Success"] = "Kayıt silindi.";
        return RedirectToAction(nameof(Index), new { slug });
    }
}
