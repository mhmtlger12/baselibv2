using Baselib.Business.DTOs;
using BaseLib.Presentation.Services.Api;
namespace BaseLib.Presentation.Extensions;
public static class ApiServiceExtensions
{
    public static IServiceCollection AddManagementApi(this IServiceCollection services)
    {
        services.AddScoped<ISystemApiService, SystemApiService>();
        services.AddScoped<IPublicSliderApiService, PublicSliderApiService>();
        AddResource<SliderDto, SaveSliderDto, SaveSliderDto>(services, ApiRoutes.Sliders);
        AddResource<UserDto, CreateUserDto, UpdateUserDto>(services, ApiRoutes.Users);
        AddResource<RoleDto, CreateRoleDto, UpdateRoleDto>(services, ApiRoutes.Roles);
        AddResource<PermissionDto, CreatePermissionDto, CreatePermissionDto>(services, ApiRoutes.Permissions);
        AddResource<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>(services, ApiRoutes.Departments);
        AddResource<MenuDto, CreateMenuDto, UpdateMenuDto>(services, ApiRoutes.Menus);
        return services;
    }
    private static void AddResource<TRead, TCreate, TUpdate>(IServiceCollection services, string route) =>
        services.AddScoped<ICrudApiService<TRead, TCreate, TUpdate>>(provider => new CrudApiService<TRead, TCreate, TUpdate>(provider.GetRequiredService<IApiClient>(), route));
}
