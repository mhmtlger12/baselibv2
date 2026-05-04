using Baselib.Business.DTOs;
using Baselib.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baselib.Presentation.Pages.Admin;

public class AuditLogsModel : PageModel
{
    private readonly ApiService _apiService;

    public List<AuditLogDto> Logs { get; private set; } = new();

    public AuditLogsModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var data = await _apiService.GetAsync<List<AuditLogDto>>("/api/auditlogs");
            if (data != null)
            {
                Logs = data;
            }
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToPage("/Login");
        }
        catch
        {
            // Error handling
        }

        return Page();
    }
}
