using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class PermissionsController(ICrudApiService<PermissionDto, CreatePermissionDto, CreatePermissionDto> permissions) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await permissions.ListAsync(ct));
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = new PermissionEditModel { IsActive = true, CRUDActionType = (Baselib.Core.Enums.CRUDActionType)1 };
        if (id > 0)
        {
            var item = await permissions.GetAsync(id, ct);
            model = new() { Id = id, Name = item.Name, Code = item.Code, Description = item.Description, ControllerName = item.ControllerName, ActionName = item.ActionName, CRUDActionType = item.CRUDActionType, IsActive = item.IsActive };
        }
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(PermissionEditModel model, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(() => model.Id == 0
            ? permissions.CreateAsync(ToDto(model), ct)
            : permissions.UpdateAsync(model.Id, ToDto(model), ct))) return Saved();
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpGet] public async Task<IActionResult> Delete(int id, CancellationToken ct) => View("Delete", new DeleteModel(id, (await permissions.GetAsync(id, ct)).Name));
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => permissions.DeleteAsync(id, ct))) return Saved("Kayıt çöp kutusuna taşındı.");
        return View("Delete", new DeleteModel(id, "Kayıt"));
    }
    private static CreatePermissionDto ToDto(PermissionEditModel model) => new()
    {
        Name = model.Name, Code = model.Code, Description = model.Description,
        ControllerName = model.ControllerName, ActionName = model.ActionName,
        CRUDActionType = model.CRUDActionType, IsActive = model.IsActive
    };

    private Task LoadOptionsAsync(PermissionEditModel model, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
