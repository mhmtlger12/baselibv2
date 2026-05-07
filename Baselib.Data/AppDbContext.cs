using Microsoft.EntityFrameworkCore;
using Baselib.Entities;
using System.Linq.Expressions;

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
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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

        modelBuilder.Entity<AppSetting>()
            .HasKey(e => e.Id);

        // Dynamic Global Query Filter for IsActive
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsActive));
                var condition = Expression.Equal(property, Expression.Constant(true));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

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
            new Permission { Id = 1, Name = "Kullanıcı Listele", ControllerName = "Users", ActionName = "List", Code = "Users_Read", Description = "Kullanıcı listeleme ve görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 2, Name = "Kullanıcı Oluştur", ControllerName = "Users", ActionName = "Add", Code = "Users_Create", Description = "Kullanıcı oluşturma", CRUDActionType = 2, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 3, Name = "Kullanıcı Güncelle", ControllerName = "Users", ActionName = "Update", Code = "Users_Update", Description = "Kullanıcı güncelleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 4, Name = "Kullanıcı Sil", ControllerName = "Users", ActionName = "Delete", Code = "Users_Delete", Description = "Kullanıcı silme", CRUDActionType = 6, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 5, Name = "Rol Listele", ControllerName = "Roles", ActionName = "List", Code = "Roles_Read", Description = "Rol listeleme ve görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 6, Name = "Rol Oluştur", ControllerName = "Roles", ActionName = "Add", Code = "Roles_Create", Description = "Rol oluşturma", CRUDActionType = 2, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 7, Name = "Rol Güncelle", ControllerName = "Roles", ActionName = "Update", Code = "Roles_Update", Description = "Rol güncelleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 8, Name = "Rol Sil", ControllerName = "Roles", ActionName = "Delete", Code = "Roles_Delete", Description = "Rol silme", CRUDActionType = 6, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 9, Name = "İzin Listele", ControllerName = "Permissions", ActionName = "List", Code = "Permissions_Read", Description = "İzin listeleme ve görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 10, Name = "İzin Oluştur", ControllerName = "Permissions", ActionName = "Add", Code = "Permissions_Create", Description = "İzin oluşturma", CRUDActionType = 2, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 11, Name = "İzin Güncelle", ControllerName = "Permissions", ActionName = "Update", Code = "Permissions_Update", Description = "İzin güncelleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 12, Name = "İzin Sil", ControllerName = "Permissions", ActionName = "Delete", Code = "Permissions_Delete", Description = "İzin silme", CRUDActionType = 6, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 13, Name = "Departman Listele", ControllerName = "Departments", ActionName = "List", Code = "Departments_Read", Description = "Departman listeleme ve görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 14, Name = "Departman Oluştur", ControllerName = "Departments", ActionName = "Add", Code = "Departments_Create", Description = "Departman oluşturma", CRUDActionType = 2, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 15, Name = "Departman Güncelle", ControllerName = "Departments", ActionName = "Update", Code = "Departments_Update", Description = "Departman güncelleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 16, Name = "Departman Sil", ControllerName = "Departments", ActionName = "Delete", Code = "Departments_Delete", Description = "Departman silme", CRUDActionType = 6, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 17, Name = "Menü Listele", ControllerName = "Menus", ActionName = "List", Code = "Menus_Read", Description = "Menü listeleme ve görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 18, Name = "Menü Oluştur", ControllerName = "Menus", ActionName = "Add", Code = "Menus_Create", Description = "Menü oluşturma", CRUDActionType = 2, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 19, Name = "Menü Güncelle", ControllerName = "Menus", ActionName = "Update", Code = "Menus_Update", Description = "Menü güncelleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 20, Name = "Menü Sil", ControllerName = "Menus", ActionName = "Delete", Code = "Menus_Delete", Description = "Menü silme", CRUDActionType = 6, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 21, Name = "Ayar Listele", ControllerName = "Settings", ActionName = "List", Code = "Settings_Read", Description = "Sistem ayarlarını listeleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 22, Name = "Ayar Güncelle", ControllerName = "Settings", ActionName = "Update", Code = "Settings_Update", Description = "Sistem ayarlarını güncelleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 23, Name = "Hareket Listele", ControllerName = "AuditLogs", ActionName = "List", Code = "AuditLogs_Read", Description = "Sistem hareketlerini (logları) görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 24, Name = "Çöp Kutusu Görüntüle", ControllerName = "RecycleBin", ActionName = "List", Code = "RecycleBin_Read", Description = "Silinmiş kayıtları görüntüleme", CRUDActionType = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 25, Name = "Çöp Kutusu Geri Yükle", ControllerName = "RecycleBin", ActionName = "Restore", Code = "RecycleBin_Restore", Description = "Silinmiş kayıtları geri yükleme", CRUDActionType = 3, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 26, Name = "Rol Seçenekleri", ControllerName = "Roles", ActionName = "SelectOption", Code = "Roles_SelectOption", Description = "Rol seçim listelerini görüntüleme", CRUDActionType = 5, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 27, Name = "Departman Seçenekleri", ControllerName = "Departments", ActionName = "SelectOption", Code = "Departments_SelectOption", Description = "Departman seçim listelerini görüntüleme", CRUDActionType = 5, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Menu>().HasData(
            new Menu { Id = 1, Name = "Dashboard", Url = "/Admin", Icon = "bi-speedometer2", Order = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 2, Name = "Kullanıcılar", Url = "/Admin/Users", Icon = "bi-people", Order = 2, PermissionId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 3, Name = "Roller", Url = "/Admin/Roles", Icon = "bi-shield-check", Order = 3, PermissionId = 5, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 4, Name = "İzinler", Url = "/Admin/Permissions", Icon = "bi-key", Order = 4, PermissionId = 9, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 5, Name = "Departmanlar", Url = "/Admin/Departments", Icon = "bi-diagram-3", Order = 5, PermissionId = 13, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 6, Name = "Menüler", Url = "/Admin/Menus", Icon = "bi-menu-button", Order = 6, PermissionId = 17, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 7, Name = "Sistem Ayarları", Url = "/Admin/Settings", Icon = "bi-gear", Order = 7, PermissionId = 21, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 8, Name = "Sistem Hareketleri", Url = "/Admin/AuditLogs", Icon = "bi-activity", Order = 8, PermissionId = 23, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 9, Name = "Çöp Kutusu", Url = "/Admin/RecycleBin", Icon = "bi-trash3", Order = 9, PermissionId = 24, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        // Admin user - password: admin
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Email = "admin@baselib.com", PasswordHash = "$2b$10$br5S4nxaGpEKXOPtd/mdvuKBmNoiWHPoJ8MRF43wYnOB/JbBz2o7u", FirstName = "Admin", LastName = "User", DepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
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
            new RolePermission { RoleId = 1, PermissionId = 11 },
            new RolePermission { RoleId = 1, PermissionId = 12 },
            new RolePermission { RoleId = 1, PermissionId = 13 },
            new RolePermission { RoleId = 1, PermissionId = 14 },
            new RolePermission { RoleId = 1, PermissionId = 15 },
            new RolePermission { RoleId = 1, PermissionId = 16 },
            new RolePermission { RoleId = 1, PermissionId = 17 },
            new RolePermission { RoleId = 1, PermissionId = 18 },
            new RolePermission { RoleId = 1, PermissionId = 19 },
            new RolePermission { RoleId = 1, PermissionId = 20 },
            new RolePermission { RoleId = 1, PermissionId = 21 },
            new RolePermission { RoleId = 1, PermissionId = 22 },
            new RolePermission { RoleId = 1, PermissionId = 23 },
            new RolePermission { RoleId = 1, PermissionId = 24 },
            new RolePermission { RoleId = 1, PermissionId = 25 },
            new RolePermission { RoleId = 1, PermissionId = 26 },
            new RolePermission { RoleId = 1, PermissionId = 27 }
        );

        modelBuilder.Entity<AppSetting>().HasData(
            new AppSetting { Id = 1, Key = "SiteName", Value = "Baselib", Description = "Uygulamanın genel adı", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new AppSetting { Id = 2, Key = "MaxLoginAttempts", Value = "5", Description = "Maksimum hatalı giriş denemesi", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new AppSetting { Id = 3, Key = "MaintenanceMode", Value = "false", Description = "Sistemi bakım moduna alır", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );
    }
}
