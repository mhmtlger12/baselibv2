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
        var totalUsers = await _users.CountAsync(ignoreQueryFilters: true);
        var activeUsers = await _users.CountAsync();
        var totalRoles = await _roles.CountAsync();
        var totalDepartments = await _departments.CountAsync();

        var userRoles = await _userRoles.GetAllAsync(includes: ur => ur.Role);
        var roleDist = userRoles
            .GroupBy(ur => ur.Role.Name)
            .Select(g => new RoleDistributionDto
            {
                RoleName = g.Key,
                UserCount = g.Count()
            })
            .ToList();

        var stats = new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalRoles = totalRoles,
            TotalDepartments = totalDepartments,
            RoleDistributions = roleDist
        };

        return DataResult<DashboardStatsDto>.Ok(stats);
    }
}
