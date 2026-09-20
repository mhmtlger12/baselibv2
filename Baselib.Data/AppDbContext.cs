using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Baselib.Core.Enums;
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
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Slider> Sliders => Set<Slider>();
    public DbSet<JobListing> JobListings => Set<JobListing>();
    public DbSet<Institution> Institutions => Set<Institution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.NormalizedUsername).HasMaxLength(100).IsRequired();
            entity.Property(e => e.NormalizedEmail).HasMaxLength(254).IsRequired();
            entity.HasIndex(e => e.NormalizedUsername).IsUnique();
            entity.HasIndex(e => e.NormalizedEmail).IsUnique();
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

            // Required User/Role ilişkileri, soft-delete filtreleriyle aynı davranmalıdır.
            entity.HasQueryFilter(ur => ur.User.IsActive && ur.Role.IsActive);
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

            // Devre dışı role veya permission'a ait ilişki satırları okunmaz.
            entity.HasQueryFilter(rp => rp.Role.IsActive && rp.Permission.IsActive);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(rt => rt.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(rt => rt.FamilyId).HasMaxLength(32).IsRequired();
            entity.Property(rt => rt.IpAddress).HasMaxLength(45);
            entity.Property(rt => rt.UserAgent).HasMaxLength(512);
            entity.Property(rt => rt.RevokedReason).HasMaxLength(64);
            entity.HasIndex(rt => rt.TokenHash).IsUnique();
            entity.HasIndex(rt => new { rt.UserId, rt.FamilyId });
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Devre dışı kullanıcıya ait oturum tokenı normal sorgularda görünmez.
            entity.HasQueryFilter(rt => rt.User.IsActive);
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(setting => setting.Id);
        });

        // BaseEntity'den türeyen her kayıt için pasif verileri otomatik gizle.
        // Yeni bir BaseEntity eklendiğinde burada ek bir filtre yazılması gerekmez.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) || entityType.ClrType.IsAbstract)
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isActive = Expression.Property(parameter, nameof(BaseEntity.IsActive));
            var condition = Expression.Equal(isActive, Expression.Constant(true));
            var filter = Expression.Lambda(condition, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.Property(slider => slider.Title).HasMaxLength(200).IsRequired();
            entity.Property(slider => slider.Description).HasMaxLength(1000).IsRequired();
            entity.Property(slider => slider.ImageUrl).HasMaxLength(2048).IsRequired();
            entity.Property(slider => slider.LinkUrl).HasMaxLength(2048);
            entity.HasIndex(slider => new { slider.IsDeleted, slider.IsActive, slider.Order });
            entity.HasQueryFilter(slider => !slider.IsDeleted && slider.IsActive);

            entity.HasData(
                new Slider
                {
                    Id = 2,
                    Title = "YKS Tercih Döneminde Doğru Karar",
                    Description = "Üniversite ve bölüm bazında taban puanları ve başarı sıralamalarını karşılaştırarak tercih yap.",
                    ImageUrl = "https://images.unsplash.com/photo-1541339907198-e08756dedf3f?w=1200&h=500&fit=crop&auto=format",
                    LinkUrl = "/taban-puanlari/yks",
                    Order = 2,
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 9, 21, 0, 0, 0, DateTimeKind.Utc)
                },
                new Slider
                {
                    Id = 3,
                    Title = "DGS ile Lisans Tamamlama Fırsatları",
                    Description = "Önlisans mezunları için geçiş yapılabilecek bölümler ve güncel taban puanları burada.",
                    ImageUrl = "https://images.unsplash.com/photo-1592280771190-3e2e4d571952?w=1200&h=500&fit=crop&auto=format",
                    LinkUrl = "/taban-puanlari/dgs",
                    Order = 3,
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 9, 21, 0, 0, 0, DateTimeKind.Utc)
                });
        });

        modelBuilder.Entity<JobListing>(entity =>
        {
            entity.Property(job => job.Institution).HasMaxLength(200).IsRequired();
            entity.Property(job => job.Summary).HasMaxLength(500).IsRequired();
            entity.Property(job => job.CategoryKey).HasMaxLength(80).IsRequired();
            entity.Property(job => job.CategoryLabel).HasMaxLength(150).IsRequired();
            entity.Property(job => job.StartDate).HasMaxLength(80).IsRequired();
            entity.Property(job => job.EndDate).HasMaxLength(80).IsRequired();
            entity.Property(job => job.SourceUrl).HasMaxLength(2048);
            entity.Property(job => job.PdfUrl).HasMaxLength(2048);
            entity.HasIndex(job => new { job.CategoryKey, job.IsActive, job.IsDeleted });
            entity.HasOne(job => job.InstitutionEntity)
                .WithMany(institution => institution.JobListings)
                .HasForeignKey(job => job.InstitutionId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasQueryFilter(job => !job.IsDeleted && job.IsActive);
            entity.HasData(CreateJobListingSeedData());
        });

        modelBuilder.Entity<Institution>(entity =>
        {
            entity.Property(institution => institution.Name).HasMaxLength(200).IsRequired();
            entity.Property(institution => institution.LogoUrl).HasMaxLength(2048);
            entity.Property(institution => institution.WebsiteUrl).HasMaxLength(2048);
            entity.HasIndex(institution => institution.Name).IsUnique();
            entity.HasQueryFilter(institution => !institution.IsDeleted && institution.IsActive);
            entity.HasData(CreateInstitutionSeedData());
        });

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 31, Name = "Slider Listele", ControllerName = "Sliders", ActionName = "List", Code = "Sliders_Read", Description = "Slaytları listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 32, Name = "Slider Oluştur", ControllerName = "Sliders", ActionName = "Add", Code = "Sliders_Create", Description = "Slayt oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 33, Name = "Slider Güncelle", ControllerName = "Sliders", ActionName = "Update", Code = "Sliders_Update", Description = "Slayt güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 34, Name = "Slider Sil", ControllerName = "Sliders", ActionName = "Delete", Code = "Sliders_Delete", Description = "Slayt silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 35, Name = "İlan Listele", ControllerName = "Jobs", ActionName = "List", Code = "Jobs_Read", Description = "İlanları listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 36, Name = "İlan Oluştur", ControllerName = "Jobs", ActionName = "Add", Code = "Jobs_Create", Description = "İlan oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 37, Name = "İlan Güncelle", ControllerName = "Jobs", ActionName = "Update", Code = "Jobs_Update", Description = "İlan güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { Id = 38, Name = "İlan Sil", ControllerName = "Jobs", ActionName = "Delete", Code = "Jobs_Delete", Description = "İlan silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { Id = 39, Name = "Kurum Listele", ControllerName = "Institutions", ActionName = "List", Code = "Institutions_Read", Description = "Kurumları listeleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { Id = 40, Name = "Kurum Oluştur", ControllerName = "Institutions", ActionName = "Add", Code = "Institutions_Create", Description = "Kurum oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { Id = 41, Name = "Kurum Güncelle", ControllerName = "Institutions", ActionName = "Update", Code = "Institutions_Update", Description = "Kurum güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { Id = 42, Name = "Kurum Sil", ControllerName = "Institutions", ActionName = "Delete", Code = "Institutions_Delete", Description = "Kurum silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
        );
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 31 },
            new RolePermission { RoleId = 1, PermissionId = 32 },
            new RolePermission { RoleId = 1, PermissionId = 33 },
            new RolePermission { RoleId = 1, PermissionId = 34 }
            , new RolePermission { RoleId = 1, PermissionId = 35 }
            , new RolePermission { RoleId = 1, PermissionId = 36 }
            , new RolePermission { RoleId = 1, PermissionId = 37 }
            , new RolePermission { RoleId = 1, PermissionId = 38 }
            , new RolePermission { RoleId = 1, PermissionId = 39 }
            , new RolePermission { RoleId = 1, PermissionId = 40 }
            , new RolePermission { RoleId = 1, PermissionId = 41 }
            , new RolePermission { RoleId = 1, PermissionId = 42 }
        );

        // Seed Data
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Yönetici", IsPrivileged = true, IsSystemRole = true, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Yönetim", Code = "YT", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Department { Id = 2, Name = "Bilgi Teknolojileri", Code = "BT", ParentDepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Department { Id = 3, Name = "İnsan Kaynakları", Code = "IK", ParentDepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Kullanıcı Listele", ControllerName = "Users", ActionName = "List", Code = "Users_Read", Description = "Kullanıcı listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 2, Name = "Kullanıcı Oluştur", ControllerName = "Users", ActionName = "Add", Code = "Users_Create", Description = "Kullanıcı oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 3, Name = "Kullanıcı Güncelle", ControllerName = "Users", ActionName = "Update", Code = "Users_Update", Description = "Kullanıcı güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 4, Name = "Kullanıcı Sil", ControllerName = "Users", ActionName = "Delete", Code = "Users_Delete", Description = "Kullanıcı silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 5, Name = "Rol Listele", ControllerName = "Roles", ActionName = "List", Code = "Roles_Read", Description = "Rol listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 6, Name = "Rol Oluştur", ControllerName = "Roles", ActionName = "Add", Code = "Roles_Create", Description = "Rol oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 7, Name = "Rol Güncelle", ControllerName = "Roles", ActionName = "Update", Code = "Roles_Update", Description = "Rol güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 8, Name = "Rol Sil", ControllerName = "Roles", ActionName = "Delete", Code = "Roles_Delete", Description = "Rol silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 9, Name = "İzin Listele", ControllerName = "Permissions", ActionName = "List", Code = "Permissions_Read", Description = "İzin listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 10, Name = "İzin Oluştur", ControllerName = "Permissions", ActionName = "Add", Code = "Permissions_Create", Description = "İzin oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 11, Name = "İzin Güncelle", ControllerName = "Permissions", ActionName = "Update", Code = "Permissions_Update", Description = "İzin güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 12, Name = "İzin Sil", ControllerName = "Permissions", ActionName = "Delete", Code = "Permissions_Delete", Description = "İzin silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 13, Name = "Departman Listele", ControllerName = "Departments", ActionName = "List", Code = "Departments_Read", Description = "Departman listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 14, Name = "Departman Oluştur", ControllerName = "Departments", ActionName = "Add", Code = "Departments_Create", Description = "Departman oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 15, Name = "Departman Güncelle", ControllerName = "Departments", ActionName = "Update", Code = "Departments_Update", Description = "Departman güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 16, Name = "Departman Sil", ControllerName = "Departments", ActionName = "Delete", Code = "Departments_Delete", Description = "Departman silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 17, Name = "Menü Listele", ControllerName = "Menus", ActionName = "List", Code = "Menus_Read", Description = "Menü listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 18, Name = "Menü Oluştur", ControllerName = "Menus", ActionName = "Add", Code = "Menus_Create", Description = "Menü oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 19, Name = "Menü Güncelle", ControllerName = "Menus", ActionName = "Update", Code = "Menus_Update", Description = "Menü güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 20, Name = "Menü Sil", ControllerName = "Menus", ActionName = "Delete", Code = "Menus_Delete", Description = "Menü silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 21, Name = "Ayar Listele", ControllerName = "Settings", ActionName = "List", Code = "Settings_Read", Description = "Sistem ayarlarını listeleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 22, Name = "Ayar Güncelle", ControllerName = "Settings", ActionName = "Update", Code = "Settings_Update", Description = "Sistem ayarlarını güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 23, Name = "Hareket Listele", ControllerName = "AuditLogs", ActionName = "List", Code = "AuditLogs_Read", Description = "Sistem hareketlerini (logları) görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 24, Name = "Çöp Kutusu Görüntüle", ControllerName = "RecycleBin", ActionName = "List", Code = "RecycleBin_Read", Description = "Silinmiş kayıtları görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 25, Name = "Çöp Kutusu Geri Yükle", ControllerName = "RecycleBin", ActionName = "Restore", Code = "RecycleBin_Restore", Description = "Silinmiş kayıtları geri yükleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 26, Name = "Rol Seçenekleri", ControllerName = "Roles", ActionName = "SelectOption", Code = "Roles_SelectOption", Description = "Rol seçim listelerini görüntüleme", CRUDActionType = CRUDActionType.Option, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 27, Name = "Departman Seçenekleri", ControllerName = "Departments", ActionName = "SelectOption", Code = "Departments_SelectOption", Description = "Departman seçim listelerini görüntüleme", CRUDActionType = CRUDActionType.Option, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 28, Name = "Dashboard Görüntüle", ControllerName = "Dashboard", ActionName = "GetStats", Code = "Dashboard_Read", Description = "Dashboard istatistiklerini görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 29, Name = "Kullanıcı Rolü Ata", ControllerName = "Users", ActionName = "AssignRoles", Code = "Users_AssignRoles", Description = "Kullanıcılara normal rol atama", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { Id = 30, Name = "Kritik Kullanıcı Rolü Ata", ControllerName = "Users", ActionName = "AssignPrivilegedRoles", Code = "Users_AssignPrivilegedRoles", Description = "Kullanıcılara kritik rol atama", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<Menu>().HasData(
            new Menu { Id = 1, Name = "Dashboard", Url = "/Admin", Icon = "bi-speedometer2", Order = 1, PermissionId = 28, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 2, Name = "Kullanıcılar", Url = "/Admin/Users", Icon = "bi-people", Order = 2, PermissionId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 3, Name = "Roller", Url = "/Admin/Roles", Icon = "bi-shield-check", Order = 3, PermissionId = 5, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 4, Name = "İzinler", Url = "/Admin/Permissions", Icon = "bi-key", Order = 4, PermissionId = 9, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 5, Name = "Departmanlar", Url = "/Admin/Departments", Icon = "bi-diagram-3", Order = 5, PermissionId = 13, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 6, Name = "Menüler", Url = "/Admin/Menus", Icon = "bi-menu-button", Order = 6, PermissionId = 17, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 7, Name = "Sistem Ayarları", Url = "/Admin/Settings", Icon = "bi-gear", Order = 7, PermissionId = 21, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 8, Name = "Sistem Hareketleri", Url = "/Admin/AuditLogs", Icon = "bi-activity", Order = 8, PermissionId = 23, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 9, Name = "Çöp Kutusu", Url = "/Admin/RecycleBin", Icon = "bi-trash3", Order = 9, PermissionId = 24, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Menu { Id = 10, Name = "İlanlar", Url = "/Admin/Jobs", Icon = "bi-megaphone", Order = 10, PermissionId = 35, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Menu { Id = 11, Name = "Kurumlar", Url = "/Admin/Institutions", Icon = "bi-building", Order = 11, PermissionId = 39, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
        );

        // Admin user - password: admin
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", NormalizedUsername = "ADMIN", Email = "admin@baselib.com", NormalizedEmail = "ADMIN@BASELIB.COM", PasswordHash = "$2b$10$br5S4nxaGpEKXOPtd/mdvuKBmNoiWHPoJ8MRF43wYnOB/JbBz2o7u", FirstName = "Admin", LastName = "User", DepartmentId = 1, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
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
            new RolePermission { RoleId = 1, PermissionId = 27 },
            new RolePermission { RoleId = 1, PermissionId = 28 },
            new RolePermission { RoleId = 1, PermissionId = 29 },
            new RolePermission { RoleId = 1, PermissionId = 30 }
        );

        modelBuilder.Entity<AppSetting>().HasData(
            new AppSetting { Id = 2, Key = "MaxLoginAttempts", Value = "5", Description = "Maksimum hatalı giriş denemesi", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
        );
    }

    private static IReadOnlyList<JobListing> CreateJobListingSeedData()
    {
        var categories = new (string Key, string Label, string[] Institutions, string Summary)[]
        {
            ("memur-a", "A Grubu Memur (Kariyer Meslek)", ["Kamu Denetçiliği Kurumu", "Rekabet Kurumu", "Gelir İdaresi Başkanlığı", "Ticaret Bakanlığı", "Devlet Arşivleri Başkanlığı"], "Uzman yardımcısı alacak"),
            ("memur-b", "B Grubu Memur", ["Adalet Bakanlığı", "Aile ve Sosyal Hizmetler Bakanlığı", "Çevre, Şehircilik ve İklim Değişikliği Bakanlığı", "Karayolları Genel Müdürlüğü", "Tapu ve Kadastro Genel Müdürlüğü"], "Memur alacak"),
            ("sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", ["Sermaye Piyasası Kurulu", "Bankacılık Düzenleme ve Denetleme Kurumu", "Türkiye İstatistik Kurumu", "Kamu İhale Kurumu", "Enerji Piyasaları Düzenleme Kurumu"], "Uzman personel alacak"),
            ("sozlesmeli-4b", "4/B Sözleşmeli Personel", ["Gençlik ve Spor Bakanlığı", "Sağlık Bakanlığı", "Aile ve Sosyal Hizmetler Bakanlığı", "Tarım ve Orman Bakanlığı", "Ulaştırma ve Altyapı Bakanlığı"], "Sözleşmeli personel alacak"),
            ("sozlesmeli-kit", "KİT Sözleşmeli Personel", ["Türkiye Elektrik İletim A.Ş.", "Eti Maden İşletmeleri", "Türkiye Petrolleri A.O.", "Devlet Demiryolları Taşımacılık A.Ş.", "Boru Hatları ile Petrol Taşıma A.Ş."], "Sözleşmeli personel alacak"),
            ("sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", ["Sivas Bilim ve Teknoloji Üniversitesi", "Devlet Su İşleri Genel Müdürlüğü", "TÜBİTAK", "Karadeniz Teknik Üniversitesi", "Kültür ve Turizm Bakanlığı"], "Sözleşmeli personel alacak"),
            ("akademik-uye", "Öğretim Üyesi", ["Ankara Üniversitesi", "Ege Üniversitesi", "Marmara Üniversitesi", "Selçuk Üniversitesi", "Bursa Uludağ Üniversitesi"], "Öğretim üyesi alacak"),
            ("akademik-gorevli", "Öğretim Görevlisi", ["Karabük Üniversitesi", "Hacettepe Üniversitesi", "Ondokuz Mayıs Üniversitesi", "Çukurova Üniversitesi", "Erciyes Üniversitesi"], "Öğretim görevlisi alacak"),
            ("akademik-arastirma", "Araştırma Görevlisi", ["Tokat Gaziosmanpaşa Üniversitesi", "Boğaziçi Üniversitesi", "İstanbul Üniversitesi", "Akdeniz Üniversitesi", "Kocaeli Üniversitesi"], "Araştırma görevlisi alacak"),
            ("isci-kariyer", "Kariyer İşçi", ["Türkiye Taşkömürü Kurumu", "Türkiye Kömür İşletmeleri", "Orman Genel Müdürlüğü", "Devlet Malzeme Ofisi", "Makina ve Kimya Endüstrisi"], "Kariyer işçi alacak"),
            ("isci-surekli", "Sürekli İşçi", ["Karayolları Genel Müdürlüğü", "Devlet Su İşleri Genel Müdürlüğü", "Türkiye Şeker Fabrikaları", "Toprak Mahsulleri Ofisi", "Belediye İştirakleri Genel Müdürlüğü"], "Sürekli işçi alacak"),
            ("isci-gecici", "Geçici İşçi", ["Tarım İşletmeleri Genel Müdürlüğü", "Orman Genel Müdürlüğü", "Türkiye İstatistik Kurumu", "Devlet Demiryolları", "Kıyı Emniyeti Genel Müdürlüğü"], "Geçici işçi alacak"),
            ("isci-engelli", "Engelli İşçi", ["Türkiye Elektrik İletim A.Ş.", "Posta ve Telgraf Teşkilatı", "Devlet Hava Meydanları İşletmesi", "Türkiye Cumhuriyeti Devlet Demiryolları", "İller Bankası"], "Engelli işçi alacak"),
            ("isci-eski-hukumlu", "Eski Hükümlü İşçi", ["Karayolları Genel Müdürlüğü", "Orman Genel Müdürlüğü", "Belediye Başkanlığı", "Devlet Su İşleri Genel Müdürlüğü", "Türkiye Kömür İşletmeleri"], "Eski hükümlü işçi alacak"),
            ("askeri", "Askeri Personel", ["Jandarma Genel Komutanlığı", "Milli Savunma Bakanlığı", "Kara Kuvvetleri Komutanlığı", "Deniz Kuvvetleri Komutanlığı", "Hava Kuvvetleri Komutanlığı"], "Personel temin edilecektir"),
            ("yargi", "Yargı Mensubu (Hakim - Savcı)", ["Adalet Bakanlığı", "Hâkimler ve Savcılar Kurulu", "Yargıtay Başkanlığı", "Danıştay Başkanlığı", "Türkiye Adalet Akademisi"], "Yargı personeli alacak")
        };
        var listings = new List<JobListing>();
        var id = 1;
        var published = new DateTime(2026, 9, 21);
        foreach (var category in categories)
        {
            for (var index = 0; index < category.Institutions.Length; index++)
            {
                listings.Add(new JobListing
                {
                    Id = id++, Institution = category.Institutions[index], Summary = category.Summary,
                    CategoryKey = category.Key, CategoryLabel = category.Label,
                    PublishedAt = published.AddDays(-(index + 1)), StartDate = "21 Eylül", EndDate = "5 Ekim",
                    SourceUrl = "https://kamuilan.sbb.gov.tr/", PdfUrl = "https://kamuilan.sbb.gov.tr/",
                    IsActive = true, CreatedDate = published.AddDays(-(index + 1))
                });
            }
        }
        return listings;
    }

    private static IReadOnlyList<Institution> CreateInstitutionSeedData() =>
        CreateJobListingSeedData().Select((job, index) => new { job.Institution, Index = index + 1 })
            .GroupBy(x => x.Institution)
            .Select(group => new Institution
            {
                Id = group.First().Index,
                Name = group.Key,
                IsActive = true,
                CreatedDate = new DateTime(2026, 9, 21)
            })
            .ToList();
}
