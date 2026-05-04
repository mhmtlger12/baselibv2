using Baselib.Business.DTOs;
using Baselib.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Baselib.Presentation.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;

    public int UserCount { get; private set; }
    public int RoleCount { get; private set; }
    public int PermissionCount { get; private set; }
    public int DepartmentCount { get; private set; }
    public int UserRoleRatio { get; private set; }
    public int PermissionCoverage { get; private set; }
    public int DepartmentCoverage { get; private set; }
    public string? LoadError { get; private set; }

    public IndexModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var users = await _apiService.GetAsync<List<UserDto>>("/api/users") ?? new();
            var roles = await _apiService.GetAsync<List<RoleDto>>("/api/roles") ?? new();
            var permissions = await _apiService.GetAsync<List<PermissionDto>>("/api/permissions") ?? new();
            var departments = await _apiService.GetAsync<List<DepartmentDto>>("/api/departments") ?? new();

            UserCount = users.Count;
            RoleCount = roles.Count;
            PermissionCount = permissions.Count;
            DepartmentCount = departments.Count;

            UserRoleRatio = ClampPercent(RoleCount == 0 ? 0 : UserCount * 100 / Math.Max(UserCount + RoleCount, 1));
            PermissionCoverage = ClampPercent(PermissionCount * 5);
            DepartmentCoverage = ClampPercent(DepartmentCount * 25);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToPage("/Login");
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
        {
            LoadError = "Dashboard verileri alınamadı. API çalışıyor mu ve oturum geçerli mi kontrol edin.";
        }

        return Page();
    }

    private static int ClampPercent(int value)
    {
        return Math.Clamp(value, 0, 100);
    }
}
