using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseLib.Presentation.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Content/slider")]
public sealed class SlidersController(
    ICrudApiService<SliderDto, SaveSliderDto, SaveSliderDto> sliders) : AdminController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await sliders.ListAsync(ct));

    [HttpGet("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        if (id == 0) return View(new SliderEditModel());
        var slider = await sliders.GetAsync(id, ct);
        return View(new SliderEditModel
        {
            Id = slider.Id, Title = slider.Title, Description = slider.Description,
            ImageUrl = slider.ImageUrl, LinkUrl = slider.LinkUrl,
            Order = slider.Order, IsActive = slider.IsActive
        });
    }

    [HttpPost("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, SliderEditModel model, CancellationToken ct)
    {
        model.Id = id;
        if (!ModelState.IsValid) return View(model);
        var saved = await ExecuteAsync(() => id == 0
            ? sliders.CreateAsync(model, ct)
            : sliders.UpdateAsync(id, model, ct));
        return saved ? Saved("Slayt kaydedildi.") : View(model);
    }

    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        View("Delete", new DeleteModel(id, (await sliders.GetAsync(id, ct)).Title));

    [HttpPost("Delete/{id:int}"), ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        var slider = await sliders.GetAsync(id, ct);
        if (await ExecuteAsync(() => sliders.DeleteAsync(id, ct))) return Saved("Slayt silindi.");
        return View("Delete", new DeleteModel(id, slider.Title));
    }
}
