using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseLib.Presentation.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Jobs")]
public sealed class JobsController(
    ICrudApiService<JobListingDto, SaveJobListingDto, SaveJobListingDto> jobs,
    ICrudApiService<InstitutionDto, SaveInstitutionDto, SaveInstitutionDto> institutions) : AdminController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await jobs.ListAsync(ct));

    [HttpGet("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        if (id == 0) return View(new JobListingEditModel { Institutions = await institutions.ListAsync(ct) });
        var item = await jobs.GetAsync(id, ct);
        var model = ToModel(item);
        model.Institutions = await institutions.ListAsync(ct);
        return View(model);
    }

    [HttpPost("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, JobListingEditModel model, CancellationToken ct)
    {
        model.Id = id;
        model.Institutions = await institutions.ListAsync(ct);

        // The form supplies only the ID; resolve the required API name on the server.
        ModelState.Remove(nameof(model.Institution));
        model.Institution = string.Empty;
        if (model.InstitutionId is int institutionId)
        {
            var institution = model.Institutions.FirstOrDefault(x => x.Id == institutionId && x.IsActive);
            if (institution is not null) model.Institution = institution.Name;
        }
        else if (id != 0)
        {
            // Preserve legacy listings that have a name but no institution relationship.
            var existing = await jobs.GetAsync(id, ct);
            if (existing.InstitutionId is null) model.Institution = existing.Institution;
        }

        if (string.IsNullOrWhiteSpace(model.Institution))
            ModelState.AddModelError(nameof(model.InstitutionId), "Lütfen geçerli ve aktif bir kurum seçin.");

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
        if (await ExecuteAsync(() => jobs.DeleteAsync(id, ct))) return Saved("İlan çöp kutusuna taşındı.");
        var item = await jobs.GetAsync(id, ct);
        return View("Delete", new DeleteModel(id, item.Institution));
    }

    private static JobListingEditModel ToModel(JobListingDto item) => new()
    {
        Id = item.Id, InstitutionId = item.InstitutionId, Institution = item.Institution, Summary = item.Summary,
        CategoryKey = item.CategoryKey, CategoryLabel = item.CategoryLabel,
        PublishedAt = item.PublishedAt, StartDate = item.StartDate, EndDate = item.EndDate,
        SourceUrl = item.SourceUrl, PdfUrl = item.PdfUrl, IsActive = item.IsActive
    };
}
