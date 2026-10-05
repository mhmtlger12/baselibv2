using Baselib.Core.Constants;
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
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Slider> Sliders => Set<Slider>();
    public DbSet<JobCategory> JobCategories => Set<JobCategory>();
    public DbSet<JobListing> JobListings => Set<JobListing>();
    public DbSet<Institution> Institutions => Set<Institution>();

    public DbSet<SiteNavigation> SiteNavigationItems => Set<SiteNavigation>();
    public DbSet<ExamCountdown> ExamCountdowns => Set<ExamCountdown>();
    public DbSet<SiteAdvertisement> SiteAdvertisements => Set<SiteAdvertisement>();
    public DbSet<SiteNews> SiteNewsItems => Set<SiteNews>();
    public DbSet<ScoreCategory> ScoreCategories => Set<ScoreCategory>();
    public DbSet<StudyLevel> StudyLevels => Set<StudyLevel>();
    public DbSet<StudyProgram> StudyPrograms => Set<StudyProgram>();
    public DbSet<ScorePeriod> ScorePeriods => Set<ScorePeriod>();
    public DbSet<ScoreEntry> ScoreEntries => Set<ScoreEntry>();
    public DbSet<PublicComment> PublicComments => Set<PublicComment>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<InformationPage> InformationPages => Set<InformationPage>();


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
            entity.HasQueryFilter(ur => ur.User.IsActive && !ur.User.IsDeleted && ur.Role.IsActive && !ur.Role.IsDeleted);
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
            entity.HasQueryFilter(rp => rp.Role.IsActive && !rp.Role.IsDeleted && rp.Permission.IsActive && !rp.Permission.IsDeleted);
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.Property(session => session.FamilyId).HasMaxLength(32).IsRequired();
            entity.Property(session => session.RevokedReason).HasMaxLength(64);
            entity.HasQueryFilter(session => session.User.IsActive && !session.User.IsDeleted);
            entity.HasIndex(session => session.FamilyId).IsUnique();
            entity.HasIndex(session => new { session.UserId, session.RevokedDate });
            entity.HasOne(session => session.User).WithMany().HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.HasQueryFilter(rt => rt.User.IsActive && !rt.User.IsDeleted);
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(setting => setting.Id);
        });

        ConfigureSiteContent(modelBuilder);

        // BaseEntity'den türeyen her kayıt için pasif verileri otomatik gizle.
        // Yeni bir BaseEntity eklendiğinde burada ek bir filtre yazılması gerekmez.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) || entityType.ClrType.IsAbstract)
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isActive = Expression.Property(parameter, nameof(BaseEntity.IsActive));
            Expression condition = Expression.Equal(isActive, Expression.Constant(true));
            if (typeof(SoftDeleteEntity).IsAssignableFrom(entityType.ClrType))
                condition = Expression.AndAlso(condition, Expression.Not(Expression.Property(parameter, nameof(SoftDeleteEntity.IsDeleted))));
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

        modelBuilder.Entity<JobCategory>(entity =>
        {
            entity.Property(category => category.Key).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Label).HasMaxLength(150).IsRequired();
            entity.Property(category => category.ParentKey).HasMaxLength(80);
            entity.HasIndex(category => category.Key).IsUnique();
            entity.HasQueryFilter(category => category.IsActive && !category.IsDeleted);
            entity.HasData(
                new JobCategory { Id = 1, Key = "memur", Label = "Memur", ParentKey = null, SortOrder = 1, IsSelectable = false, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 2, Key = "memur-a", Label = "A Grubu Memur (Kariyer Meslek)", ParentKey = "memur", SortOrder = 0, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 3, Key = "memur-b", Label = "B Grubu Memur", ParentKey = "memur", SortOrder = 1, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 4, Key = "sozlesmeli", Label = "Sözleşmeli Personel", ParentKey = null, SortOrder = 2, IsSelectable = false, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 5, Key = "sozlesmeli-kariyer", Label = "Kariyer Sözleşmeli Personel", ParentKey = "sozlesmeli", SortOrder = 0, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 6, Key = "sozlesmeli-4b", Label = "4/B Sözleşmeli Personel", ParentKey = "sozlesmeli", SortOrder = 1, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 7, Key = "sozlesmeli-kit", Label = "KİT Sözleşmeli Personel", ParentKey = "sozlesmeli", SortOrder = 2, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 8, Key = "sozlesmeli-kurumsal", Label = "Kurumsal Sözleşmeli Personel", ParentKey = "sozlesmeli", SortOrder = 3, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 9, Key = "akademik", Label = "Akademik Personel", ParentKey = null, SortOrder = 3, IsSelectable = false, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 10, Key = "akademik-uye", Label = "Öğretim Üyesi", ParentKey = "akademik", SortOrder = 0, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 11, Key = "akademik-gorevli", Label = "Öğretim Görevlisi", ParentKey = "akademik", SortOrder = 1, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 12, Key = "akademik-arastirma", Label = "Araştırma Görevlisi", ParentKey = "akademik", SortOrder = 2, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 13, Key = "isci", Label = "İşçi", ParentKey = null, SortOrder = 4, IsSelectable = false, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 14, Key = "isci-kariyer", Label = "Kariyer İşçi", ParentKey = "isci", SortOrder = 0, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 15, Key = "isci-surekli", Label = "Sürekli İşçi", ParentKey = "isci", SortOrder = 1, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 16, Key = "isci-gecici", Label = "Geçici İşçi", ParentKey = "isci", SortOrder = 2, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 17, Key = "isci-engelli", Label = "Engelli İşçi", ParentKey = "isci", SortOrder = 3, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 18, Key = "isci-eski-hukumlu", Label = "Eski Hükümlü İşçi", ParentKey = "isci", SortOrder = 4, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 19, Key = "askeri", Label = "Askeri Personel", ParentKey = null, SortOrder = 5, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) },
                new JobCategory { Id = 20, Key = "yargi", Label = "Yargı Mensubu (Hakim - Savcı)", ParentKey = null, SortOrder = 6, IsSelectable = true, CreatedDate = new DateTime(2026, 9, 21) });
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
            new Permission { IsSystem = true, Id = 43, Name = "Kullanıcı Parolası Sıfırla", ControllerName = "Users", ActionName = "ResetPassword", Code = "Users_ResetPassword", Description = "Kullanıcı parolasını sıfırlama", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 27) },
            new Permission { IsSystem = true, Id = 44, Name = "Ayrıcalıklı Hesap Parolası Sıfırla", ControllerName = "Users", ActionName = "ResetPrivilegedPassword", Code = "Users_ResetPrivilegedPassword", Description = "Ayrıcalıklı hesap parolasını sıfırlama; kullanıcı parolası sıfırlama izni de gerekir", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 27) },
            new Permission { IsSystem = true, Id = 31, Name = "Slider Listele", ControllerName = "Sliders", ActionName = "List", Code = "Sliders_Read", Description = "Slaytları listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 32, Name = "Slider Oluştur", ControllerName = "Sliders", ActionName = "Add", Code = "Sliders_Create", Description = "Slayt oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 33, Name = "Slider Güncelle", ControllerName = "Sliders", ActionName = "Update", Code = "Sliders_Update", Description = "Slayt güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 34, Name = "Slider Sil", ControllerName = "Sliders", ActionName = "Delete", Code = "Sliders_Delete", Description = "Slayt silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 35, Name = "İlan Listele", ControllerName = "Jobs", ActionName = "List", Code = "Jobs_Read", Description = "İlanları listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 36, Name = "İlan Oluştur", ControllerName = "Jobs", ActionName = "Add", Code = "Jobs_Create", Description = "İlan oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 37, Name = "İlan Güncelle", ControllerName = "Jobs", ActionName = "Update", Code = "Jobs_Update", Description = "İlan güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) },
            new Permission { IsSystem = true, Id = 38, Name = "İlan Sil", ControllerName = "Jobs", ActionName = "Delete", Code = "Jobs_Delete", Description = "İlan silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { IsSystem = true, Id = 39, Name = "Kurum Listele", ControllerName = "Institutions", ActionName = "List", Code = "Institutions_Read", Description = "Kurumları listeleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { IsSystem = true, Id = 40, Name = "Kurum Oluştur", ControllerName = "Institutions", ActionName = "Add", Code = "Institutions_Create", Description = "Kurum oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { IsSystem = true, Id = 41, Name = "Kurum Güncelle", ControllerName = "Institutions", ActionName = "Update", Code = "Institutions_Update", Description = "Kurum güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
            , new Permission { IsSystem = true, Id = 42, Name = "Kurum Sil", ControllerName = "Institutions", ActionName = "Delete", Code = "Institutions_Delete", Description = "Kurum silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2026, 9, 21) }
        );
        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = 1, PermissionId = 43 },
            new RolePermission { RoleId = 1, PermissionId = 44 },
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
            new Permission { IsSystem = true, Id = 1, Name = "Kullanıcı Listele", ControllerName = "Users", ActionName = "List", Code = "Users_Read", Description = "Kullanıcı listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 2, Name = "Kullanıcı Oluştur", ControllerName = "Users", ActionName = "Add", Code = "Users_Create", Description = "Kullanıcı oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 3, Name = "Kullanıcı Güncelle", ControllerName = "Users", ActionName = "Update", Code = "Users_Update", Description = "Kullanıcı güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 4, Name = "Kullanıcı Sil", ControllerName = "Users", ActionName = "Delete", Code = "Users_Delete", Description = "Kullanıcı silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 5, Name = "Rol Listele", ControllerName = "Roles", ActionName = "List", Code = "Roles_Read", Description = "Rol listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 6, Name = "Rol Oluştur", ControllerName = "Roles", ActionName = "Add", Code = "Roles_Create", Description = "Rol oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 7, Name = "Rol Güncelle", ControllerName = "Roles", ActionName = "Update", Code = "Roles_Update", Description = "Rol güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 8, Name = "Rol Sil", ControllerName = "Roles", ActionName = "Delete", Code = "Roles_Delete", Description = "Rol silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 9, Name = "İzin Listele", ControllerName = "Permissions", ActionName = "List", Code = "Permissions_Read", Description = "İzin listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 10, Name = "İzin Oluştur", ControllerName = "Permissions", ActionName = "Add", Code = "Permissions_Create", Description = "İzin oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 11, Name = "İzin Güncelle", ControllerName = "Permissions", ActionName = "Update", Code = "Permissions_Update", Description = "İzin güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 12, Name = "İzin Sil", ControllerName = "Permissions", ActionName = "Delete", Code = "Permissions_Delete", Description = "İzin silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 13, Name = "Departman Listele", ControllerName = "Departments", ActionName = "List", Code = "Departments_Read", Description = "Departman listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 14, Name = "Departman Oluştur", ControllerName = "Departments", ActionName = "Add", Code = "Departments_Create", Description = "Departman oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 15, Name = "Departman Güncelle", ControllerName = "Departments", ActionName = "Update", Code = "Departments_Update", Description = "Departman güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 16, Name = "Departman Sil", ControllerName = "Departments", ActionName = "Delete", Code = "Departments_Delete", Description = "Departman silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 17, Name = "Menü Listele", ControllerName = "Menus", ActionName = "List", Code = "Menus_Read", Description = "Menü listeleme ve görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 18, Name = "Menü Oluştur", ControllerName = "Menus", ActionName = "Add", Code = "Menus_Create", Description = "Menü oluşturma", CRUDActionType = CRUDActionType.Add, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 19, Name = "Menü Güncelle", ControllerName = "Menus", ActionName = "Update", Code = "Menus_Update", Description = "Menü güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 20, Name = "Menü Sil", ControllerName = "Menus", ActionName = "Delete", Code = "Menus_Delete", Description = "Menü silme", CRUDActionType = CRUDActionType.Delete, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 21, Name = "Ayar Listele", ControllerName = "Settings", ActionName = "List", Code = "Settings_Read", Description = "Sistem ayarlarını listeleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 22, Name = "Ayar Güncelle", ControllerName = "Settings", ActionName = "Update", Code = "Settings_Update", Description = "Sistem ayarlarını güncelleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 23, Name = "Hareket Listele", ControllerName = "AuditLogs", ActionName = "List", Code = "AuditLogs_Read", Description = "Sistem hareketlerini (logları) görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 24, Name = "Çöp Kutusu Görüntüle", ControllerName = "RecycleBin", ActionName = "List", Code = "RecycleBin_Read", Description = "Silinmiş kayıtları görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 25, Name = "Çöp Kutusu Geri Yükle", ControllerName = "RecycleBin", ActionName = "Restore", Code = "RecycleBin_Restore", Description = "Silinmiş kayıtları geri yükleme", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 26, Name = "Rol Seçenekleri", ControllerName = "Roles", ActionName = "SelectOption", Code = "Roles_SelectOption", Description = "Rol seçim listelerini görüntüleme", CRUDActionType = CRUDActionType.Option, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 27, Name = "Departman Seçenekleri", ControllerName = "Departments", ActionName = "SelectOption", Code = "Departments_SelectOption", Description = "Departman seçim listelerini görüntüleme", CRUDActionType = CRUDActionType.Option, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 28, Name = "Dashboard Görüntüle", ControllerName = "Dashboard", ActionName = "GetStats", Code = "Dashboard_Read", Description = "Dashboard istatistiklerini görüntüleme", CRUDActionType = CRUDActionType.View, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 29, Name = "Kullanıcı Rolü Ata", ControllerName = "Users", ActionName = "AssignRoles", Code = "Users_AssignRoles", Description = "Kullanıcılara normal rol atama", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new Permission { IsSystem = true, Id = 30, Name = "Kritik Kullanıcı Rolü Ata", ControllerName = "Users", ActionName = "AssignPrivilegedRoles", Code = "Users_AssignPrivilegedRoles", Description = "Kullanıcılara kritik rol atama", CRUDActionType = CRUDActionType.Update, IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
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
            new AppSetting { Id = 2, Key = "MaxLoginAttempts", Value = "5", Description = "Maksimum hatalı giriş denemesi", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
            new AppSetting { Id = 3, Key = SiteSettingConstants.Name, Value = "puannokta", Description = "Site adı (en fazla 100 karakter)", IsActive = true, CreatedDate = new DateTime(2026, 10, 4) },
            new AppSetting { Id = 4, Key = SiteSettingConstants.Tagline, Value = "Taban puanları, tek noktada.", Description = "Site sloganı (en fazla 200 karakter)", IsActive = true, CreatedDate = new DateTime(2026, 10, 4) }
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

    private static void ConfigureSiteContent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SiteNavigation>().ToTable("SiteNavigation");
        modelBuilder.Entity<ExamCountdown>().ToTable("ExamCountdowns");
        modelBuilder.Entity<SiteAdvertisement>().ToTable("SiteAdvertisements");
        modelBuilder.Entity<SiteNews>().ToTable("SiteNews");
        modelBuilder.Entity<ScoreCategory>().ToTable("ScoreCategories").HasIndex(x => x.Key).IsUnique();
        modelBuilder.Entity<StudyLevel>().ToTable("StudyLevels").HasIndex(x => x.Key).IsUnique();
        modelBuilder.Entity<StudyProgram>().ToTable("StudyPrograms");
        modelBuilder.Entity<ScorePeriod>().ToTable("ScorePeriods").HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<ScoreEntry>().ToTable("ScoreEntries");
        modelBuilder.Entity<PublicComment>().ToTable("PublicComments");
        modelBuilder.Entity<ContactMessage>().ToTable("ContactMessages");
        modelBuilder.Entity<InformationPage>().ToTable("InformationPages").HasIndex(x => x.Slug).IsUnique();
        modelBuilder.Entity<StudyProgram>().HasOne(x => x.StudyLevel).WithMany().HasForeignKey(x => x.StudyLevelId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ScoreEntry>().HasOne(x => x.ScoreCategory).WithMany().HasForeignKey(x => x.ScoreCategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ScoreEntry>().HasOne(x => x.StudyProgram).WithMany().HasForeignKey(x => x.StudyProgramId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ScoreEntry>().HasOne(x => x.ScorePeriod).WithMany().HasForeignKey(x => x.ScorePeriodId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ScoreEntry>().Property(x => x.MinScore).HasPrecision(10, 5);
        modelBuilder.Entity<ScoreEntry>().Property(x => x.MaxScore).HasPrecision(10, 5);
        modelBuilder.Entity<ScoreEntry>().HasIndex(x => new { x.ScoreCategoryId, x.StudyProgramId, x.ScorePeriodId });
        modelBuilder.Entity<PublicComment>().HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PublicComment>().HasIndex(x => new { x.Page, x.Status });
        modelBuilder.Entity<SiteNavigation>().HasData(
            new SiteNavigation { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Label = "Ana Sayfa", Url = "/", SortOrder = 0 },
            new SiteNavigation { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Label = "İlanlar", Url = "/ilanlar", SortOrder = 1 },
            new SiteNavigation { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Label = "KPSS", Url = "/taban-puanlari/kpss-lisans", SortOrder = 2 },
            new SiteNavigation { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Label = "YKS", Url = "/taban-puanlari/yks", SortOrder = 3 },
            new SiteNavigation { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Label = "ALES", Url = "/bilgi/ales", SortOrder = 4 },
            new SiteNavigation { Id = 6, CreatedDate = new DateTime(2026, 9, 21), Label = "DGS", Url = "/taban-puanlari/dgs", SortOrder = 5 },
            new SiteNavigation { Id = 7, CreatedDate = new DateTime(2026, 9, 21), Label = "İletişim", Url = "/iletisim", SortOrder = 6 });
        modelBuilder.Entity<ExamCountdown>().HasData(
            new ExamCountdown { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Label = "YKS", Target = new DateTime(2026, 6, 20, 10, 15, 0), SortOrder = 0 },
            new ExamCountdown { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Label = "KPSS Ortaöğretim", Target = new DateTime(2026, 9, 13, 10, 15, 0), SortOrder = 1 },
            new ExamCountdown { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Label = "KPSS Önlisans", Target = new DateTime(2026, 9, 6, 10, 15, 0), SortOrder = 2 },
            new ExamCountdown { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Label = "KPSS Lisans", Target = new DateTime(2026, 7, 19, 10, 15, 0), SortOrder = 3 },
            new ExamCountdown { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Label = "DGS", Target = new DateTime(2026, 7, 5, 10, 15, 0), SortOrder = 4 },
            new ExamCountdown { Id = 6, CreatedDate = new DateTime(2026, 9, 21), Label = "ALES", Target = new DateTime(2026, 5, 10, 10, 15, 0), SortOrder = 5 });
        modelBuilder.Entity<SiteAdvertisement>().HasData(
            new SiteAdvertisement { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Name = "Anasayfa Üst Banner", Position = "home", ImageUrl = "https://images.unsplash.com/photo-1557804506-669a67965ba0?w=400&h=250&fit=crop", LinkUrl = "https://reklam.example.com", IsActive = true },
            new SiteAdvertisement { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Name = "Sidebar Reklamı", Position = "sidebar", ImageUrl = "", LinkUrl = "https://reklam.example.com", IsActive = true });
        modelBuilder.Entity<SiteNews>().HasData(
            new SiteNews { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "KPSS 2024/2 Atama Taban Puanları", PublishedAt = new DateTime(2024, 9, 18), Link = "/taban-puanlari/kpss-lisans", SortOrder = 0 },
            new SiteNews { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "Bilgisayar Mühendisliği YKS Sıralamaları", PublishedAt = new DateTime(2024, 9, 16), Link = "/taban-puanlari/kpss-lisans", SortOrder = 1 },
            new SiteNews { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "Adalet Öğretmenliği Atama Puanları", PublishedAt = new DateTime(2024, 9, 14), Link = "/taban-puanlari/kpss-lisans", SortOrder = 2 },
            new SiteNews { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "DGS Hemşirelik Geçiş Puanları", PublishedAt = new DateTime(2024, 9, 11), Link = "/taban-puanlari/kpss-lisans", SortOrder = 3 },
            new SiteNews { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Section = "osym", Title = "2024 KPSS tercih kılavuzu yayımlandı", PublishedAt = new DateTime(2024, 9, 19), Link = "https://www.osym.gov.tr", SortOrder = 0 },
            new SiteNews { Id = 6, CreatedDate = new DateTime(2026, 9, 21), Section = "osym", Title = "ALES/3 başvuruları başladı", PublishedAt = new DateTime(2024, 9, 12), Link = "https://www.osym.gov.tr", SortOrder = 1 },
            new SiteNews { Id = 7, CreatedDate = new DateTime(2026, 9, 21), Section = "osym", Title = "YKS ek yerleştirme takvimi açıklandı", PublishedAt = new DateTime(2024, 9, 5), Link = "https://www.osym.gov.tr", SortOrder = 2 },
            new SiteNews { Id = 8, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "Bilgisayar Mühendisliği 2024 Taban Puanı", Category = "YKS", Link = "/taban-puanlari/yks/bolum/5", SortOrder = 1, PublishedAt = new DateTime(2026, 9, 21), IsActive = true },
            new SiteNews { Id = 9, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "KPSS Önlisans Atama Puanları", Category = "KPSS", Link = "/taban-puanlari/kpss-onlisans", SortOrder = 2, PublishedAt = new DateTime(2026, 9, 21), IsActive = true },
            new SiteNews { Id = 10, CreatedDate = new DateTime(2026, 9, 21), Section = "recent", Title = "Hemşirelik DGS Geçiş Puanları", Category = "DGS", Link = "/taban-puanlari/dgs/bolum/7", SortOrder = 3, PublishedAt = new DateTime(2026, 9, 21), IsActive = true },
            new SiteNews { Id = 11, CreatedDate = new DateTime(2026, 9, 21), Section = "osym", Title = "2026 YKS Başvuru Kılavuzu Yayımlandı", Category = "", Link = "https://osym.gov.tr", SortOrder = 0, PublishedAt = new DateTime(2026, 2, 10), IsActive = true },
            new SiteNews { Id = 12, CreatedDate = new DateTime(2026, 9, 21), Section = "osym", Title = "KPSS 2026 Sınav Takvimi Açıklandı", Category = "", Link = "https://osym.gov.tr", SortOrder = 0, PublishedAt = new DateTime(2026, 1, 15), IsActive = true },
            new SiteNews { Id = 13, CreatedDate = new DateTime(2026, 9, 21), Section = "osym", Title = "DGS Tercih İşlemleri Başladı", Category = "", Link = "https://osym.gov.tr", SortOrder = 0, PublishedAt = new DateTime(2026, 7, 22), IsActive = false });
        modelBuilder.Entity<ScoreCategory>().HasData(
            new ScoreCategory { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Key = "kpss-lisans", Title = "KPSS Lisans", Label = "Taban Puanı", SortOrder = 0 },
            new ScoreCategory { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Key = "kpss-onlisans", Title = "KPSS Önlisans", Label = "Taban Puanı", SortOrder = 1 },
            new ScoreCategory { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Key = "kpss-orta", Title = "KPSS Ortaöğretim", Label = "Taban Puanı", SortOrder = 2 },
            new ScoreCategory { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Key = "yks", Title = "YKS", Label = "Taban Puanı", SortOrder = 3 },
            new ScoreCategory { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Key = "dgs", Title = "DGS", Label = "Taban Puanı", SortOrder = 4 });
        modelBuilder.Entity<StudyLevel>().HasData(
            new StudyLevel { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Key = "lisans", Label = "Lisans", SortOrder = 0 },
            new StudyLevel { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Key = "onlisans", Label = "Önlisans", SortOrder = 1 },
            new StudyLevel { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Key = "ortaogretim", Label = "Ortaöğretim", SortOrder = 2 });
        modelBuilder.Entity<StudyProgram>().HasData(
            new StudyProgram { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Name = "Acil Yardım ve Afet Yönetimi", StudyLevelId = 1 },
            new StudyProgram { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Name = "Adalet Öğretmenliği", StudyLevelId = 1 },
            new StudyProgram { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Name = "Aktüerya", StudyLevelId = 1 },
            new StudyProgram { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Name = "Alman Dili ve Edebiyatı", StudyLevelId = 1 },
            new StudyProgram { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Name = "Bilgisayar Mühendisliği", StudyLevelId = 1 },
            new StudyProgram { Id = 6, CreatedDate = new DateTime(2026, 9, 21), Name = "Elektrik-Elektronik Mühendisliği", StudyLevelId = 1 },
            new StudyProgram { Id = 7, CreatedDate = new DateTime(2026, 9, 21), Name = "Hemşirelik", StudyLevelId = 1 },
            new StudyProgram { Id = 8, CreatedDate = new DateTime(2026, 9, 21), Name = "Hukuk", StudyLevelId = 1 },
            new StudyProgram { Id = 9, CreatedDate = new DateTime(2026, 9, 21), Name = "İşletme", StudyLevelId = 1 },
            new StudyProgram { Id = 10, CreatedDate = new DateTime(2026, 9, 21), Name = "Psikoloji", StudyLevelId = 1 },
            new StudyProgram { Id = 11, CreatedDate = new DateTime(2026, 9, 21), Name = "Adalet", StudyLevelId = 2 },
            new StudyProgram { Id = 12, CreatedDate = new DateTime(2026, 9, 21), Name = "Bilgisayar Programcılığı", StudyLevelId = 2 },
            new StudyProgram { Id = 13, CreatedDate = new DateTime(2026, 9, 21), Name = "Büro Yönetimi ve Yönetici Asistanlığı", StudyLevelId = 2 },
            new StudyProgram { Id = 14, CreatedDate = new DateTime(2026, 9, 21), Name = "İlk ve Acil Yardım", StudyLevelId = 2 },
            new StudyProgram { Id = 15, CreatedDate = new DateTime(2026, 9, 21), Name = "Muhasebe ve Vergi Uygulamaları", StudyLevelId = 2 },
            new StudyProgram { Id = 16, CreatedDate = new DateTime(2026, 9, 21), Name = "Tıbbi Dokümantasyon ve Sekreterlik", StudyLevelId = 2 },
            new StudyProgram { Id = 17, CreatedDate = new DateTime(2026, 9, 21), Name = "Büro Memurluğu", StudyLevelId = 3 },
            new StudyProgram { Id = 18, CreatedDate = new DateTime(2026, 9, 21), Name = "Veri Hazırlama ve Kontrol İşletmeni", StudyLevelId = 3 },
            new StudyProgram { Id = 19, CreatedDate = new DateTime(2026, 9, 21), Name = "Zabıt Katipliği", StudyLevelId = 3 },
            new StudyProgram { Id = 20, CreatedDate = new DateTime(2026, 9, 21), Name = "Hizmetli", StudyLevelId = 3 },
            new StudyProgram { Id = 21, CreatedDate = new DateTime(2026, 9, 21), Name = "Büro Yönetimi", StudyLevelId = 2 },
            new StudyProgram { Id = 22, CreatedDate = new DateTime(2026, 9, 21), Name = "Tıp", StudyLevelId = 1 });
        modelBuilder.Entity<ScorePeriod>().HasData(
            new ScorePeriod { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Name = "2024/2", SortOrder = 0 },
            new ScorePeriod { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Name = "2024/1", SortOrder = 1 },
            new ScorePeriod { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Name = "2023/2", SortOrder = 2 },
            new ScorePeriod { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Name = "2023/1", SortOrder = 3 },
            new ScorePeriod { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Name = "2024", SortOrder = 4 });
        modelBuilder.Entity<ScoreEntry>().HasData(
            new ScoreEntry { Id = 1, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Adalet Bakanlığı", City = "Ankara", Title = "Mühendis", Quota = 12, Vacant = 0, MinScore = 80.95434m, MaxScore = 89.21876m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 2, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Sağlık Bakanlığı", City = "İstanbul", Title = "Mühendis", Quota = 8, Vacant = 0, MinScore = 80.99622m, MaxScore = 81.21216m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 3, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Milli Eğitim Bakanlığı", City = "İzmir", Title = "Mühendis", Quota = 5, Vacant = 1, MinScore = 78.4512m, MaxScore = 84.7719m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 4, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Çevre, Şehircilik ve İklim Değişikliği Bakanlığı", City = "Bursa", Title = "Mühendis", Quota = 6, Vacant = 0, MinScore = 79.1033m, MaxScore = 83.4102m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 5, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Hazine ve Maliye Bakanlığı", City = "Ankara", Title = "Mühendis", Quota = 10, Vacant = 0, MinScore = 82.331m, MaxScore = 90.0021m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 6, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Ulaştırma ve Altyapı Bakanlığı", City = "Antalya", Title = "Mühendis", Quota = 4, Vacant = 2, MinScore = 76.8801m, MaxScore = 82.1109m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 7, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Gençlik ve Spor Bakanlığı", City = "Konya", Title = "Mühendis", Quota = 3, Vacant = 0, MinScore = 77.5522m, MaxScore = 80.9931m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 8, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Tarım ve Orman Bakanlığı", City = "Samsun", Title = "Mühendis", Quota = 7, Vacant = 1, MinScore = 75.2201m, MaxScore = 81.4407m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 9, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "İçişleri Bakanlığı", City = "Ankara", Title = "Mühendis", Quota = 9, Vacant = 0, MinScore = 83.7712m, MaxScore = 91.2298m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 10, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 1, StudyProgramId = 5, ScorePeriodId = 1, Institution = "Sanayi ve Teknoloji Bakanlığı", City = "Kocaeli", Title = "Mühendis", Quota = 5, Vacant = 0, MinScore = 79.9012m, MaxScore = 85.5533m, Qualification = "3607 - Bilgisayar Müh." },
            new ScoreEntry { Id = 11, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 2, StudyProgramId = 12, ScorePeriodId = 5, Institution = "Sağlık Bakanlığı", MinScore = 88.42m, MaxScore = 88.42m, Rank = 1240, IsActive = true },
            new ScoreEntry { Id = 12, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 2, StudyProgramId = 21, ScorePeriodId = 5, Institution = "Adalet Bakanlığı", MinScore = 82.15m, MaxScore = 82.15m, Rank = 3980, IsActive = true },
            new ScoreEntry { Id = 13, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 5, StudyProgramId = 5, ScorePeriodId = 5, Institution = "İTÜ", MinScore = 342.8m, MaxScore = 342.8m, Rank = 210, IsActive = true },
            new ScoreEntry { Id = 14, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 5, StudyProgramId = 7, ScorePeriodId = 5, Institution = "Ege Üniversitesi", MinScore = 318.5m, MaxScore = 318.5m, Rank = 640, IsActive = true },
            new ScoreEntry { Id = 15, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 4, StudyProgramId = 22, ScorePeriodId = 5, Institution = "Hacettepe Üniversitesi", MinScore = 541.2m, MaxScore = 541.2m, Rank = 850, IsActive = true },
            new ScoreEntry { Id = 16, CreatedDate = new DateTime(2026, 9, 21), ScoreCategoryId = 4, StudyProgramId = 8, ScorePeriodId = 5, Institution = "Ankara Üniversitesi", MinScore = 498.6m, MaxScore = 498.6m, Rank = 5400, IsActive = true });
        modelBuilder.Entity<PublicComment>().HasData(
            new PublicComment { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Author = "Ahmet Y.", Body = "KPSS lisans için bu puanlar yeterli mi? Bilgisayar mühendisliği düşünüyorum.", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Onaylı", Likes = 3, ParentId = null },
            new PublicComment { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Author = "Ayşe K.", Body = "Geçen dönem bu puanla atananlar oldu, ama kadro sayısına da bakmak lazım.", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Onaylı", Likes = 1, ParentId = 1 },
            new PublicComment { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Author = "Ahmet Y.", Body = "Teşekkürler, kadro sayısına bakayım.", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Onaylı", Likes = 0, ParentId = 2 },
            new PublicComment { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Author = "Mehmet T.", Body = "Kurum tercihine göre değişir. Ankara dışını da işaretlersen şansın artar.", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Onaylı", Likes = 2, ParentId = 1 },
            new PublicComment { Id = 5, CreatedDate = new DateTime(2026, 9, 21), Author = "Zeynep D.", Body = "Sağlık Bakanlığı kadrolarında taban puan biraz daha düşük görünüyor.", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Onaylı", Likes = 4, ParentId = null },
            new PublicComment { Id = 6, CreatedDate = new DateTime(2026, 9, 18), Author = "Ahmet Y.", Body = "KPSS lisans için bu puanlar yeterli mi?", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Onaylı" },
            new PublicComment { Id = 7, CreatedDate = new DateTime(2026, 9, 19), Author = "Zeynep D.", Body = "Sağlık Bakanlığı kadrolarında taban puan daha düşük görünüyor.", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Beklemede" },
            new PublicComment { Id = 8, CreatedDate = new DateTime(2026, 9, 20), Author = "guest_4821", Body = "ucuz takipçi -> bit.ly/xxx", Page = "/taban-puanlari/kpss-lisans/bolum/5", Status = "Spam" });
        modelBuilder.Entity<ContactMessage>().HasData(
            new ContactMessage { Id = 1, CreatedDate = new DateTime(2026, 9, 20), Name = "Elif Şahin", Email = "elif@example.com", Subject = "Puan hesaplama hakkında", Message = "Merhaba, KPSS puan hesaplamasında...", Status = "Yeni" },
            new ContactMessage { Id = 2, CreatedDate = new DateTime(2026, 9, 17), Name = "Burak Demir", Email = "burak@example.com", Subject = "Reklam iş birliği", Message = "Sitenizde reklam vermek istiyorum.", Status = "Okundu" });
        modelBuilder.Entity<InformationPage>().HasData(
            new InformationPage { Id = 1, CreatedDate = new DateTime(2026, 9, 21), Slug = "hakkimizda", Title = "Hakkımızda", Body = "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur." },
            new InformationPage { Id = 2, CreatedDate = new DateTime(2026, 9, 21), Slug = "gizlilik", Title = "Gizlilik Politikası", Body = "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur." },
            new InformationPage { Id = 3, CreatedDate = new DateTime(2026, 9, 21), Slug = "kullanim-kosullari", Title = "Kullanım Koşulları", Body = "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur." },
            new InformationPage { Id = 4, CreatedDate = new DateTime(2026, 9, 21), Slug = "ales", Title = "ALES", Body = "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur." });
        modelBuilder.Entity<Permission>().HasData(new Permission { Id = 45, Name = "Site İçeriği Görüntüle", ControllerName = "Content", ActionName = "Read", Code = "Content_Read", IsSystem = true, CRUDActionType = CRUDActionType.View, CreatedDate = new DateTime(2026, 9, 21) });
        modelBuilder.Entity<RolePermission>().HasData(new RolePermission { RoleId = 1, PermissionId = 45 });
        modelBuilder.Entity<Permission>().HasData(new Permission { Id = 46, Name = "Site İçeriği Oluştur", ControllerName = "Content", ActionName = "Create", Code = "Content_Create", IsSystem = true, CRUDActionType = CRUDActionType.Add, CreatedDate = new DateTime(2026, 9, 21) });
        modelBuilder.Entity<RolePermission>().HasData(new RolePermission { RoleId = 1, PermissionId = 46 });
        modelBuilder.Entity<Permission>().HasData(new Permission { Id = 47, Name = "Site İçeriği Güncelle", ControllerName = "Content", ActionName = "Update", Code = "Content_Update", IsSystem = true, CRUDActionType = CRUDActionType.Update, CreatedDate = new DateTime(2026, 9, 21) });
        modelBuilder.Entity<RolePermission>().HasData(new RolePermission { RoleId = 1, PermissionId = 47 });
        modelBuilder.Entity<Permission>().HasData(new Permission { Id = 48, Name = "Site İçeriği Sil", ControllerName = "Content", ActionName = "Delete", Code = "Content_Delete", IsSystem = true, CRUDActionType = CRUDActionType.Delete, CreatedDate = new DateTime(2026, 9, 21) });
        modelBuilder.Entity<RolePermission>().HasData(new RolePermission { RoleId = 1, PermissionId = 48 });
    }
}
