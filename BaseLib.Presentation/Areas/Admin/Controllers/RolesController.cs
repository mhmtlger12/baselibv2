using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class RolesController(ICrudApiService<RoleDto, CreateRoleDto, UpdateRoleDto> roles,
    ICrudApiService<PermissionDto, CreatePermissionDto, CreatePermissionDto> permissions) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await roles.ListAsync(ct));
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = new RoleEditModel { IsActive = true };
        if (id > 0)
        {
            var item = await roles.GetAsync(id, ct);
            model = new() { Id = id, Name = item.Name, Description = item.Description, IsActive = item.IsActive, PermissionIds = item.Permissions.Select(x => x.Id).ToList() };
        }
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(RoleEditModel model, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(() => model.Id == 0
            ? roles.CreateAsync(new CreateRoleDto { Name = model.Name, Description = model.Description, PermissionIds = model.PermissionIds }, ct)
            : roles.UpdateAsync(model.Id, new UpdateRoleDto
            { Name = model.Name, Description = model.Description, PermissionIds = model.PermissionIds, IsActive = model.IsActive }, ct))) return Saved();
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpGet] public async Task<IActionResult> Delete(int id, CancellationToken ct) => View("Delete", new DeleteModel(id, (await roles.GetAsync(id, ct)).Name));
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => roles.DeleteAsync(id, ct))) return Saved("Kayıt çöp kutusuna taşındı.");
        return View("Delete", new DeleteModel(id, "Kayıt"));
    }
    private async Task LoadOptionsAsync(RoleEditModel model, CancellationToken ct)
    {
        model.AvailablePermissions = await permissions.ListAsync(ct);
    }
}
