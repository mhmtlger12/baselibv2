using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class DepartmentsController(ICrudApiService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto> departments) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await departments.ListAsync(ct));
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = new DepartmentEditModel { IsActive = true };
        if (id > 0)
        {
            var item = await departments.GetAsync(id, ct);
            model = new() { Id = id, Name = item.Name, Code = item.Code, ParentDepartmentId = item.ParentDepartmentId, IsActive = item.IsActive };
        }
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(DepartmentEditModel model, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(() => model.Id == 0
            ? departments.CreateAsync(new CreateDepartmentDto { Name = model.Name, Code = model.Code, ParentDepartmentId = model.ParentDepartmentId }, ct)
            : departments.UpdateAsync(model.Id, new UpdateDepartmentDto
            { Name = model.Name, Code = model.Code, ParentDepartmentId = model.ParentDepartmentId, IsActive = model.IsActive }, ct))) return Saved();
        await LoadOptionsAsync(model, ct);
        return View(model);
    }
    [HttpGet] public async Task<IActionResult> Delete(int id, CancellationToken ct) => View("Delete", new DeleteModel(id, (await departments.GetAsync(id, ct)).Name));
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => departments.DeleteAsync(id, ct))) return Saved("Kayıt çöp kutusuna taşındı.");
        return View("Delete", new DeleteModel(id, "Kayıt"));
    }
    private async Task LoadOptionsAsync(DepartmentEditModel model, CancellationToken ct)
    {
        model.Departments = (await departments.ListAsync(ct)).Where(x => x.Id != model.Id).ToList();
    }
}
