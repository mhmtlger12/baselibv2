using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Baselib.Presentation.Services;

namespace Baselib.Presentation.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public string ErrorMessage { get; set; } = "";

    private readonly AuthService _authService;

    public LoginModel(AuthService authService)
    {
        _authService = authService;
    }

    public IActionResult OnGet()
    {
        if (_authService.IsAuthenticated)
        {
            return RedirectToPage("/Admin/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Kullanıcı adı ve şifre gereklidir";
            return Page();
        }

        var result = await _authService.LoginAsync(Username, Password);

        if (result == null)
        {
            ErrorMessage = "Kullanıcı adı veya şifre hatalı";
            return Page();
        }

        return RedirectToPage("/Admin/Index");
    }
}