using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<Role> _roles;
    private readonly IRepository<Department> _departments;
    private readonly IRepository<UserRole> _userRoles;

    public DashboardService(
        IRepository<User> users,
        IRepository<Role> roles,
        IRepository<Department> departments,
        IRepository<UserRole> userRoles)
    {
        _users = users;
        _roles = roles;
        _departments = departments;
        _userRoles = userRoles;
    }

    public async Task<IDataResult<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalUsers = await _users.CountAsync(ignoreQueryFilters: true, cancellationToken: cancellationToken);
        var activeUsers = await _users.CountAsync(cancellationToken: cancellationToken);
        var totalRoles = await _roles.CountAsync(cancellationToken: cancellationToken);
        var totalDepartments = await _departments.CountAsync(cancellationToken: cancellationToken);

        // GroupBy, SQL'e çevrilir; tüm UserRole kayıtları uygulama belleğine alınmaz.
        var roleDist = await _userRoles.SelectAsync(
            query => query
                .GroupBy(userRole => userRole.Role.Name)
                .Select(group => new RoleDistributionDto
                {
                    RoleName = group.Key,
                    UserCount = group.Count()
                }),
            cancellationToken);

        var stats = new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalRoles = totalRoles,
            TotalDepartments = totalDepartments,
            RoleDistributions = roleDist.ToList()
        };

        return DataResult<DashboardStatsDto>.Ok(stats);
    }
}
