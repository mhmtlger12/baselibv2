using Microsoft.Extensions.DependencyInjection;
using Baselib.Business.Interfaces;
using Baselib.Business.Mappings;
using Baselib.Business.Services;

namespace Baselib.Business.Extensions;

public static class BusinessServiceExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPermissionCheckService, PermissionCheckService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<IJobListingService, JobListingService>();
        services.AddScoped<IInstitutionService, InstitutionService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IRecycleBinService, RecycleBinService>();

        return services;
    }
}
