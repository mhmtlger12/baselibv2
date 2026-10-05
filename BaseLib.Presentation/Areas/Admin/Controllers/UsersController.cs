using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class UsersController(
    ICrudApiService<UserDto, CreateUserDto, UpdateUserDto> users,
    ICrudApiService<RoleDto, CreateRoleDto, UpdateRoleDto> roles,
    ICrudApiService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto> departments,
    ISystemApiService system) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await users.ListAsync(ct));
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = new UserEditModel { IsActive = true };
        if (id > 0)
        {
            var user = await users.GetAsync(id, ct);
            model = new() { Id = id, Username = user.Username, Email = user.Email, FirstName = user.FirstName,
                LastName = user.LastName, Phone = user.Phone, DepartmentId = user.DepartmentId, IsActive = user.IsActive };
        }
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(UserEditModel model, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(async () =>
        {
            if (model.Id == 0)
                await users.CreateAsync(new CreateUserDto { Username = model.Username, Email = model.Email, Password = model.Password!,
                    FirstName = model.FirstName, LastName = model.LastName, Phone = model.Phone, DepartmentId = model.DepartmentId }, ct);
            else
            {
                await users.UpdateAsync(model.Id, new UpdateUserDto
                {
                    Username = model.Username, Email = model.Email,
                    FirstName = model.FirstName, LastName = model.LastName, Phone = model.Phone,
                    DepartmentId = model.DepartmentId, IsActive = model.IsActive
                }, ct);
            }
        })) return Saved(model.Id == 0 ? "Kullanıcı oluşturuldu. Rol atamak için Roller ekranını açın." : "Kullanıcı bilgileri kaydedildi.");
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Roles(int id, CancellationToken ct)
    {
        var user = await users.GetAsync(id, ct);
        return View(new UserRolesModel
        {
            Id = id, Username = user.Username, RoleIds = user.RoleIds,
            AvailableRoles = await roles.ListSelectOptionsAsync(ct)
        });
    }
    [HttpPost]
    public async Task<IActionResult> Roles(int id, UserRolesModel model, CancellationToken ct)
    {
        model.Id = id;
        if (ModelState.IsValid && await ExecuteAsync(() => system.AssignRolesAsync(id, model.RoleIds, ct)))
            return Saved("Kullanıcının rolleri kaydedildi.");
        model.Username = (await users.GetAsync(id, ct)).Username;
        model.AvailableRoles = await roles.ListSelectOptionsAsync(ct);
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> ResetPassword(int id, CancellationToken ct)
    {
        var user = await users.GetAsync(id, ct);
        return View(new UserPasswordResetModel { Id = id, Username = user.Username });
    }
    [HttpPost]
    public async Task<IActionResult> ResetPassword(int id, UserPasswordResetModel model, CancellationToken ct)
    {
        model.Id = id;
        if (ModelState.IsValid && await ExecuteAsync(() => system.ResetUserPasswordAsync(id, model, ct)))
            return Saved("Kullanıcının şifresi sıfırlandı.");
        model.Username = (await users.GetAsync(id, ct)).Username;
        return View(model);
    }
    [HttpGet] public async Task<IActionResult> Delete(int id, CancellationToken ct) => View("Delete", new DeleteModel(id, (await users.GetAsync(id, ct)).Username));
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => users.DeleteAsync(id, ct))) return Saved("Kullanıcı çöp kutusuna taşındı.");
        return View("Delete", new DeleteModel(id, "Kullanıcı"));
    }
    private async Task LoadOptionsAsync(UserEditModel model, CancellationToken ct)
    {
        model.Departments = await departments.ListSelectOptionsAsync(ct);
    }
}
