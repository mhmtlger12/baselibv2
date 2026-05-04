using Microsoft.EntityFrameworkCore;
using Baselib.Entities;

namespace Baselib.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Users)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasOne(e => e.ParentDepartment)
                  .WithMany(e => e.SubDepartments)
                  .HasForeignKey(e => e.ParentDepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.SubMenus)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Permission)
                  .WithMany(p => p.Menus)
                  .HasForeignKey(e => e.PermissionId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Data
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Yönetici", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Yönetim", Code = "YT", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Department { Id = 2, Name = "Bilgi Teknolojileri", Code = "BT", ParentDepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Department { Id = 3, Name = "İnsan Kaynakları", Code = "IK", ParentDepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Kullanıcı Listesi", ControllerName = "Users", ActionName = "Index", Code = "Users_Index", Description = "Kullanıcı Listesi", CRUDActionType = 15, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 2, Name = "Kullanıcı Oluştur", ControllerName = "Users", ActionName = "Create", Code = "Users_Create", Description = "Kullanıcı Oluştur", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 3, Name = "Kullanıcı Güncelle", ControllerName = "Users", ActionName = "Update", Code = "Users_Update", Description = "Kullanıcı Güncelle", CRUDActionType = 4, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 4, Name = "Kullanıcı Sil", ControllerName = "Users", ActionName = "Delete", Code = "Users_Delete", Description = "Kullanıcı Sil", CRUDActionType = 8, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 5, Name = "Rol Listesi", ControllerName = "Roles", ActionName = "Index", Code = "Roles_Index", Description = "Rol Listesi", CRUDActionType = 15, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 6, Name = "Rol Oluştur", ControllerName = "Roles", ActionName = "Create", Code = "Roles_Create", Description = "Rol Oluştur", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 7, Name = "Rol Güncelle", ControllerName = "Roles", ActionName = "Update", Code = "Roles_Update", Description = "Rol Güncelle", CRUDActionType = 4, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 8, Name = "Rol Sil", ControllerName = "Roles", ActionName = "Delete", Code = "Roles_Delete", Description = "Rol Sil", CRUDActionType = 8, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 9, Name = "İzin Listesi", ControllerName = "Permissions", ActionName = "Index", Code = "Permissions_Index", Description = "İzin Listesi", CRUDActionType = 15, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 10, Name = "Departman Listesi", ControllerName = "Departments", ActionName = "Index", Code = "Departments_Index", Description = "Departman Listesi", CRUDActionType = 15, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 11, Name = "Menü Listesi", ControllerName = "Menus", ActionName = "Index", Code = "Menus_Index", Description = "Menü Listesi", CRUDActionType = 15, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Menu>().HasData(
            new Menu { Id = 1, Name = "Dashboard", Url = "/Admin", Icon = "bi-speedometer2", Order = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 2, Name = "Kullanıcılar", Url = "/Admin/Users", Icon = "bi-people", Order = 2, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 3, Name = "Roller", Url = "/Admin/Roles", Icon = "bi-shield-check", Order = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 4, Name = "İzinler", Url = "/Admin/Permissions", Icon = "bi-key", Order = 4, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 5, Name = "Departmanlar", Url = "/Admin/Departments", Icon = "bi-diagram-3", Order = 5, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 6, Name = "Menüler", Url = "/Admin/Menus", Icon = "bi-menu-button", Order = 6, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        // Admin user - password: admin
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Email = "admin@baselib.com", PasswordHash = "$2a$10$N9qo8uLOickgx2ZMRZoMye4kDgR8KD6CWJ5C3u0s0p0s0s0s0s0s0u", FirstName = "Admin", LastName = "User", DepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = 1, RoleId = 1 }
        );

        // Admin role has all permissions
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },
            new RolePermission { RoleId = 1, PermissionId = 9 },
            new RolePermission { RoleId = 1, PermissionId = 10 },
            new RolePermission { RoleId = 1, PermissionId = 11 }
        );
    }
}