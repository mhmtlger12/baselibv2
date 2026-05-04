using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Baselib.Presentation.Pages.Admin.Roles;

public class IndexModel : PageModel
{
    public List<RoleDto> Roles { get; set; } = new();

    public async Task OnGetAsync()
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7001");
        
        var response = await client.GetAsync("/api/Roles/List");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<RoleDto>>>();
            Roles = result?.Data ?? new();
        }
    }
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
    public bool IsActive { get; set; }
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int StatusCode { get; set; }
    public T? Data { get; set; }
}