using Baselib.Business.DTOs;
using Baselib.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baselib.Presentation.Pages.Admin;

public class RecycleBinModel : PageModel
{
    private readonly ApiService _apiService;

    public List<RecycleBinItemDto> Items { get; private set; } = new();

    public RecycleBinModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var data = await _apiService.GetAsync<List<RecycleBinItemDto>>("/api/recyclebin");
            if (data != null)
            {
                Items = data;
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
