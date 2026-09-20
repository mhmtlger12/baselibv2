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
                LastName = user.LastName, Phone = user.Phone, DepartmentId = user.DepartmentId, RoleIds = user.RoleIds, IsActive = user.IsActive };
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
                    FirstName = model.FirstName, LastName = model.LastName, Phone = model.Phone, DepartmentId = model.DepartmentId, RoleIds = model.RoleIds }, ct);
            else
            {
                await users.UpdateAsync(model.Id, new UpdateUserDto
                {
                    Username = model.Username, Email = model.Email, Password = model.Password,
                    FirstName = model.FirstName, LastName = model.LastName, Phone = model.Phone,
                    DepartmentId = model.DepartmentId, IsActive = model.IsActive
                }, ct);
                try { await system.AssignRolesAsync(model.Id, model.RoleIds, ct); }
                catch (ApiException error) when (error.StatusCode != 401)
                { throw new ApiException(error.StatusCode == 403 ? 400 : error.StatusCode, "Kullanıcı bilgileri kaydedildi; rol atamaları kaydedilemedi. " + error.Message); }
            }
        })) return Saved();
        await LoadOptionsAsync(model, ct);
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
        model.AvailableRoles = await roles.ListAsync(ct);
        model.Departments = await departments.ListAsync(ct);
    }
}
