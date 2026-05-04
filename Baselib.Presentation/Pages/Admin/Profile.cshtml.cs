using Baselib.Business.DTOs;
using Baselib.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baselib.Presentation.Pages.Admin;

public class ProfileModel : PageModel
{
    private readonly ApiService _apiService;

    public UserDto? UserProfile { get; private set; }
    public string? LoadError { get; private set; }

    public ProfileModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            UserProfile = await _apiService.GetAsync<UserDto>("/api/profile");
            
            if (UserProfile == null)
            {
                LoadError = "Profil bilgileri bulunamadı.";
            }
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToPage("/Login");
        }
        catch (Exception)
        {
            LoadError = "Profil verileri alınamadı. Lütfen tekrar giriş yapın.";
        }

        return Page();
    }
}
