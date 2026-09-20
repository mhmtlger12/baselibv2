using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseLib.Presentation.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Institutions")]
public sealed class InstitutionsController(
    ICrudApiService<InstitutionDto, SaveInstitutionDto, SaveInstitutionDto> institutions) : AdminController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await institutions.ListAsync(ct));

    [HttpGet("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        if (id == 0) return View(new InstitutionEditModel());
        var item = await institutions.GetAsync(id, ct);
        return View(new InstitutionEditModel { Id = item.Id, Name = item.Name, LogoUrl = item.LogoUrl, WebsiteUrl = item.WebsiteUrl, IsActive = item.IsActive });
    }

    [HttpPost("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, InstitutionEditModel model, CancellationToken ct)
    {
        model.Id = id;
        if (!ModelState.IsValid) return View(model);
        var saved = await ExecuteAsync(() => id == 0 ? institutions.CreateAsync(model, ct) : institutions.UpdateAsync(id, model, ct));
        return saved ? Saved("Kurum kaydedildi.") : View(model);
    }

    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await institutions.GetAsync(id, ct);
        return View(new DeleteModel(id, item.Name));
    }

    [HttpPost("Delete/{id:int}"), ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => institutions.DeleteAsync(id, ct))) return Saved("Kurum silindi.");
        var item = await institutions.GetAsync(id, ct);
        return View("Delete", new DeleteModel(id, item.Name));
    }
}
