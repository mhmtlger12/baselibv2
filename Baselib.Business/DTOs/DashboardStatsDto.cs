namespace Baselib.Business.DTOs;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRoles { get; set; }
    public int TotalDepartments { get; set; }
    public List<RoleDistributionDto> RoleDistributions { get; set; } = new();
}

public class RoleDistributionDto
{
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
}
