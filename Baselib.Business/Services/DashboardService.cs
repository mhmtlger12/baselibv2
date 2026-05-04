using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalUsers = await _users.Query().CountAsync(cancellationToken);
        var activeUsers = await _users.Query().CountAsync(u => u.IsActive, cancellationToken);
        var totalRoles = await _roles.Query().CountAsync(cancellationToken);
        var totalDepartments = await _departments.Query().CountAsync(cancellationToken);

        var roleDist = await _userRoles.Query()
            .Include(ur => ur.Role)
            .GroupBy(ur => ur.Role.Name)
            .Select(g => new RoleDistributionDto
            {
                RoleName = g.Key,
                UserCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalRoles = totalRoles,
            TotalDepartments = totalDepartments,
            RoleDistributions = roleDist
        };
    }
}
