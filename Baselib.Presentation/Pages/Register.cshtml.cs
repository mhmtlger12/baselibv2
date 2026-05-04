using Baselib.Business.DTOs;
using Baselib.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baselib.Presentation.Pages;

public class RegisterModel : PageModel
{
    private readonly AuthService _authService;

    [BindProperty]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    [TempData]
    public string? StatusMessage { get; set; }

    public RegisterModel(AuthService authService)
    {
        _authService = authService;
    }

    public IActionResult OnGet()
    {
        if (_authService.IsAuthenticated())
            return RedirectToPage("/Admin/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Tüm zorunlu alanları doldurun";
            return Page();
        }

        var created = await _authService.RegisterAsync(new CreateUserDto
        {
            FirstName = FirstName,
            LastName = LastName,
            Username = Username,
            Email = Email,
            Password = Password
        });

        if (!created)
        {
            ErrorMessage = "Kayıt oluşturulamadı";
            return Page();
        }

        StatusMessage = "Kayıt oluşturuldu. Giriş yapabilirsiniz.";
        return RedirectToPage("/Login");
    }
}
