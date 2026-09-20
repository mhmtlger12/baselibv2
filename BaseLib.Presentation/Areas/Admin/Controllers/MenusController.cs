using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class MenusController(ICrudApiService<MenuDto, CreateMenuDto, UpdateMenuDto> menus,
    ICrudApiService<PermissionDto, CreatePermissionDto, CreatePermissionDto> permissions) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await menus.ListAsync(ct));
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = new MenuEditModel { IsActive = true };
        if (id > 0)
        {
            var item = await menus.GetAsync(id, ct);
            model = new() { Id = id, Name = item.Name, Url = item.Url, Icon = item.Icon, ParentId = item.ParentId, Order = item.Order, PermissionId = item.PermissionId, IsActive = item.IsActive };
        }
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(MenuEditModel model, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(() => model.Id == 0
            ? menus.CreateAsync(new CreateMenuDto { Name = model.Name, Url = model.Url, Icon = model.Icon, ParentId = model.ParentId, Order = model.Order, PermissionId = model.PermissionId }, ct)
            : menus.UpdateAsync(model.Id, new UpdateMenuDto
            { Name = model.Name, Url = model.Url, Icon = model.Icon, ParentId = model.ParentId, Order = model.Order, PermissionId = model.PermissionId, IsActive = model.IsActive }, ct))) return Saved();
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpGet] public async Task<IActionResult> Delete(int id, CancellationToken ct) => View("Delete", new DeleteModel(id, (await menus.GetAsync(id, ct)).Name));
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => menus.DeleteAsync(id, ct))) return Saved("Kayıt çöp kutusuna taşındı.");
        return View("Delete", new DeleteModel(id, "Kayıt"));
    }
    private async Task LoadOptionsAsync(MenuEditModel model, CancellationToken ct)
    {
        model.Menus = (await menus.ListAsync(ct)).Where(x => x.Id != model.Id).ToList();
        model.Permissions = await permissions.ListAsync(ct);
    }
}
