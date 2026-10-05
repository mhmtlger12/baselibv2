using System.Globalization;
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
    ICrudApiService<InstitutionDto, SaveInstitutionDto, SaveInstitutionDto> institutions,
    IPublicJobApiService publicJobs) : AdminController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await jobs.ListAsync(ct));

    [HttpGet("Categories")]
    public IActionResult Categories() => RedirectToAction("Index", "Content", new { slug = "job-categories" });

    [HttpGet("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = id == 0 ? new JobListingEditModel() : ToModel(await jobs.GetAsync(id, ct));
        await LoadOptionsAsync(model, ct);
        return View(model);
    }

    [HttpPost("Edit/{id:int?}")]
    public async Task<IActionResult> Edit(int id, JobListingEditModel model, CancellationToken ct)
    {
        model.Id = id;
        await LoadOptionsAsync(model, ct);
        if (model.InstitutionId is int institutionId && !model.Institutions.Any(x => x.Id == institutionId && x.IsActive) ||
            id == 0 && model.InstitutionId is null)
            ModelState.AddModelError(nameof(model.InstitutionId), "Lütfen geçerli ve aktif bir kurum seçin.");
        if (!model.Categories.Any(x => x.Key == model.CategoryKey && x.IsSelectable))
            ModelState.AddModelError(nameof(model.CategoryKey), "Lütfen geçerli ve aktif bir ilan kategorisi seçin.");

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

    private async Task LoadOptionsAsync(JobListingEditModel model, CancellationToken ct)
    {
        model.Institutions = await institutions.ListAsync(ct);
        model.Categories = await publicJobs.GetCategoriesAsync(ct);
        if (model.Id > 0 && model.InstitutionId is null)
            model.Institution = (await jobs.GetAsync(model.Id, ct)).Institution;
    }

    private static string DateInput(string value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ? value : "";

    private static JobListingEditModel ToModel(JobListingDto item) => new()
    {
        Id = item.Id, InstitutionId = item.InstitutionId, Institution = item.Institution, Summary = item.Summary,
        CategoryKey = item.CategoryKey, CategoryLabel = item.CategoryLabel,
        PublishedAt = item.PublishedAt, StartDate = DateInput(item.StartDate), EndDate = DateInput(item.EndDate),
        LegacyDates = DateInput(item.StartDate) == "" || DateInput(item.EndDate) == ""
            ? $"Eski tarih bilgisi: {item.StartDate} – {item.EndDate}. Kaydetmeden önce yıl içeren tarihleri seçin." : null,
        SourceUrl = item.SourceUrl, PdfUrl = item.PdfUrl, IsActive = item.IsActive
    };
}
