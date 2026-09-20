namespace BaseLib.Presentation.Services.Api;
public static class ApiRoutes
{
    public const string Login = "api/auth/login";
    public const string Refresh = "api/auth/refresh";
    public const string Logout = "api/auth/logout";
    public const string Profile = "api/profile";
    public const string Password = "api/profile/password";
    public const string Dashboard = "api/dashboard/stats";
    public const string Users = "api/users";
    public const string Roles = "api/roles";
    public const string Permissions = "api/permissions";
    public const string Departments = "api/departments";
    public const string Menus = "api/menus";
    public const string Sliders = "api/sliders";
    public const string PublishedSliders = Sliders + "/published";
    public const string Jobs = "api/jobs";
    public const string PublishedJobs = Jobs + "/published";
    public const string MyMenus = "api/menus/me";
    public const string Settings = "api/settings";
    public const string AuditLogs = "api/auditlogs";
    public const string RecycleBin = "api/recyclebin";
    public static string Item(string resource, int id) => $"{resource}/{id}";
    public static string UserRoles(int id) => $"{Users}/{id}/roles";
    public static string SwitchRole(int id) => $"api/auth/switch-role/{id}";
    public static string Restore(string type, int id) => $"{RecycleBin}/{Uri.EscapeDataString(type)}/{id}/restore";
}
