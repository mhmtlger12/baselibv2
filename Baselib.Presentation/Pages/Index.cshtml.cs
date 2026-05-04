using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Baselib.Presentation.Services;

namespace Baselib.Presentation.Pages;

public class IndexModel : PageModel
{
    private readonly AuthService _authService;

    public IndexModel(AuthService authService)
    {
        _authService = authService;
    }

    public IActionResult OnGet()
    {
        return _authService.IsAuthenticated()
            ? RedirectToPage("/Admin/Index")
            : RedirectToPage("/Login");
    }
}
