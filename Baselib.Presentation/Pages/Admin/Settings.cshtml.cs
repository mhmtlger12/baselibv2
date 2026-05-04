using Baselib.Business.DTOs;
using Baselib.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baselib.Presentation.Pages.Admin;

public class SettingsModel : PageModel
{
    private readonly ApiService _apiService;

    public List<SettingDto> Settings { get; private set; } = new();

    public SettingsModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var data = await _apiService.GetAsync<List<SettingDto>>("/api/settings");
            if (data != null)
            {
                Settings = data;
            }
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToPage("/Login");
        }
        catch
        {
            // Error handling can be enhanced
        }

        return Page();
    }
}
