using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseLib.Presentation.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Jobs")]
public sealed class JobsController(
    ICrudApiService<JobListingDto, SaveJobListingDto, SaveJobListingDto> jobs) : AdminController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await jobs.ListAsync(ct));

    [HttpGet("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        if (id == 0) return View(new JobListingEditModel());
        var item = await jobs.GetAsync(id, ct);
        return View(ToModel(item));
    }

    [HttpPost("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, JobListingEditModel model, CancellationToken ct)
    {
        model.Id = id;
        if (!ModelState.IsValid) return View(model);
        var saved = await ExecuteAsync(() => id == 0 ? jobs.CreateAsync(model, ct) : jobs.UpdateAsync(id, model, ct));
        return saved ? Saved("İlan kaydedildi.") : View(model);
    }

    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await jobs.GetAsync(id, ct);
        return View(new DeleteModel(id, item.Institution));
    }

    [HttpPost("Delete/{id:int}"), ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => jobs.DeleteAsync(id, ct))) return Saved("İlan silindi.");
        var item = await jobs.GetAsync(id, ct);
        return View("Delete", new DeleteModel(id, item.Institution));
    }

    private static JobListingEditModel ToModel(JobListingDto item) => new()
    {
        Id = item.Id, Institution = item.Institution, Summary = item.Summary,
        CategoryKey = item.CategoryKey, CategoryLabel = item.CategoryLabel,
        PublishedAt = item.PublishedAt, StartDate = item.StartDate, EndDate = item.EndDate,
        SourceUrl = item.SourceUrl, PdfUrl = item.PdfUrl, IsActive = item.IsActive
    };
}
