using Baselib.Business.DTOs;
namespace BaseLib.Presentation.Services.Api;
public interface ISystemApiService
{
    Task<DashboardStatsDto> DashboardAsync(CancellationToken ct);
    Task<List<AuditLogDto>> AuditLogsAsync(CancellationToken ct);
    Task<List<RecycleBinItemDto>> RecycleBinAsync(CancellationToken ct);
    Task RestoreAsync(string type, int id, CancellationToken ct);
    Task<List<SettingDto>> SettingsAsync(CancellationToken ct);
    Task UpdateSettingAsync(int id, UpdateSettingDto input, CancellationToken ct);
    Task<UserDto> ProfileAsync(CancellationToken ct);
    Task ChangePasswordAsync(ChangePasswordDto input, CancellationToken ct);
    Task AssignRolesAsync(int id, List<int> roleIds, CancellationToken ct);
    Task<List<MenuDto>> MyMenusAsync(CancellationToken ct);
}
public sealed class SystemApiService(IApiClient api) : ISystemApiService
{
    public Task<DashboardStatsDto> DashboardAsync(CancellationToken ct) => api.GetAsync<DashboardStatsDto>(ApiRoutes.Dashboard, ct);
    public Task<List<AuditLogDto>> AuditLogsAsync(CancellationToken ct) => api.GetAsync<List<AuditLogDto>>(ApiRoutes.AuditLogs, ct);
    public Task<List<RecycleBinItemDto>> RecycleBinAsync(CancellationToken ct) => api.GetAsync<List<RecycleBinItemDto>>(ApiRoutes.RecycleBin, ct);
    public Task RestoreAsync(string type, int id, CancellationToken ct) => api.SendAsync(HttpMethod.Put, ApiRoutes.Restore(type, id), ct: ct);
    public Task<List<SettingDto>> SettingsAsync(CancellationToken ct) => api.GetAsync<List<SettingDto>>(ApiRoutes.Settings, ct);
    public Task UpdateSettingAsync(int id, UpdateSettingDto input, CancellationToken ct) => api.SendAsync(HttpMethod.Put, ApiRoutes.Item(ApiRoutes.Settings, id), new UpdateSettingDto { Value = input.Value }, ct);
    public Task<UserDto> ProfileAsync(CancellationToken ct) => api.GetAsync<UserDto>(ApiRoutes.Profile, ct);
    public Task ChangePasswordAsync(ChangePasswordDto input, CancellationToken ct) => api.SendAsync(HttpMethod.Put, ApiRoutes.Password, new ChangePasswordDto { CurrentPassword = input.CurrentPassword, NewPassword = input.NewPassword }, ct);
    public Task AssignRolesAsync(int id, List<int> roleIds, CancellationToken ct) => api.SendAsync(HttpMethod.Put, ApiRoutes.UserRoles(id), roleIds, ct);
    public Task<List<MenuDto>> MyMenusAsync(CancellationToken ct) => api.GetAsync<List<MenuDto>>(ApiRoutes.MyMenus, ct);
}
