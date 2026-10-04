using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Departments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WebsiteUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ControllerName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CRUDActionType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPrivileged = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUsername = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FirstName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JobListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InstitutionId = table.Column<int>(type: "int", nullable: true),
                    Institution = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Summary = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryKey = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryLabel = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StartDate = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EndDate = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PdfUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobListings_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Menus_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Controller = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Route = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FamilyId = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActiveRoleId = table.Column<int>(type: "int", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUsedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedReason = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "Key", "UpdatedBy", "UpdatedDate", "Value" },
                values: new object[] { 2, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maksimum hatalı giriş denemesi", true, "MaxLoginAttempts", null, null, "5" });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate", "IsActive", "Name", "ParentDepartmentId", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 1, "YT", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Yönetim", null, null, null });

            migrationBuilder.InsertData(
                table: "Institutions",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "LogoUrl", "Name", "UpdatedBy", "UpdatedDate", "WebsiteUrl" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Kamu Denetçiliği Kurumu", null, null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Rekabet Kurumu", null, null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Gelir İdaresi Başkanlığı", null, null, null },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Ticaret Bakanlığı", null, null, null },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Devlet Arşivleri Başkanlığı", null, null, null },
                    { 6, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Adalet Bakanlığı", null, null, null },
                    { 7, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Aile ve Sosyal Hizmetler Bakanlığı", null, null, null },
                    { 8, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Çevre, Şehircilik ve İklim Değişikliği Bakanlığı", null, null, null },
                    { 9, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Karayolları Genel Müdürlüğü", null, null, null },
                    { 10, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Tapu ve Kadastro Genel Müdürlüğü", null, null, null },
                    { 11, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Sermaye Piyasası Kurulu", null, null, null },
                    { 12, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Bankacılık Düzenleme ve Denetleme Kurumu", null, null, null },
                    { 13, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye İstatistik Kurumu", null, null, null },
                    { 14, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Kamu İhale Kurumu", null, null, null },
                    { 15, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Enerji Piyasaları Düzenleme Kurumu", null, null, null },
                    { 16, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Gençlik ve Spor Bakanlığı", null, null, null },
                    { 17, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Sağlık Bakanlığı", null, null, null },
                    { 19, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Tarım ve Orman Bakanlığı", null, null, null },
                    { 20, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Ulaştırma ve Altyapı Bakanlığı", null, null, null },
                    { 21, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Elektrik İletim A.Ş.", null, null, null },
                    { 22, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Eti Maden İşletmeleri", null, null, null },
                    { 23, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Petrolleri A.O.", null, null, null },
                    { 24, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Devlet Demiryolları Taşımacılık A.Ş.", null, null, null },
                    { 25, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Boru Hatları ile Petrol Taşıma A.Ş.", null, null, null },
                    { 26, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Sivas Bilim ve Teknoloji Üniversitesi", null, null, null },
                    { 27, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Devlet Su İşleri Genel Müdürlüğü", null, null, null },
                    { 28, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "TÜBİTAK", null, null, null },
                    { 29, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Karadeniz Teknik Üniversitesi", null, null, null },
                    { 30, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Kültür ve Turizm Bakanlığı", null, null, null },
                    { 31, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Ankara Üniversitesi", null, null, null },
                    { 32, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Ege Üniversitesi", null, null, null },
                    { 33, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Marmara Üniversitesi", null, null, null },
                    { 34, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Selçuk Üniversitesi", null, null, null },
                    { 35, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Bursa Uludağ Üniversitesi", null, null, null },
                    { 36, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Karabük Üniversitesi", null, null, null },
                    { 37, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Hacettepe Üniversitesi", null, null, null },
                    { 38, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Ondokuz Mayıs Üniversitesi", null, null, null },
                    { 39, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Çukurova Üniversitesi", null, null, null },
                    { 40, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Erciyes Üniversitesi", null, null, null },
                    { 41, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Tokat Gaziosmanpaşa Üniversitesi", null, null, null },
                    { 42, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Boğaziçi Üniversitesi", null, null, null },
                    { 43, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "İstanbul Üniversitesi", null, null, null },
                    { 44, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Akdeniz Üniversitesi", null, null, null },
                    { 45, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Kocaeli Üniversitesi", null, null, null },
                    { 46, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Taşkömürü Kurumu", null, null, null },
                    { 47, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Kömür İşletmeleri", null, null, null },
                    { 48, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Orman Genel Müdürlüğü", null, null, null },
                    { 49, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Devlet Malzeme Ofisi", null, null, null },
                    { 50, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Makina ve Kimya Endüstrisi", null, null, null },
                    { 53, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Şeker Fabrikaları", null, null, null },
                    { 54, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Toprak Mahsulleri Ofisi", null, null, null },
                    { 55, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Belediye İştirakleri Genel Müdürlüğü", null, null, null },
                    { 56, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Tarım İşletmeleri Genel Müdürlüğü", null, null, null },
                    { 59, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Devlet Demiryolları", null, null, null },
                    { 60, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Kıyı Emniyeti Genel Müdürlüğü", null, null, null },
                    { 62, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Posta ve Telgraf Teşkilatı", null, null, null },
                    { 63, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Devlet Hava Meydanları İşletmesi", null, null, null },
                    { 64, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Cumhuriyeti Devlet Demiryolları", null, null, null },
                    { 65, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "İller Bankası", null, null, null },
                    { 68, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Belediye Başkanlığı", null, null, null },
                    { 71, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Jandarma Genel Komutanlığı", null, null, null },
                    { 72, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Milli Savunma Bakanlığı", null, null, null },
                    { 73, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Kara Kuvvetleri Komutanlığı", null, null, null },
                    { 74, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Deniz Kuvvetleri Komutanlığı", null, null, null },
                    { 75, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Hava Kuvvetleri Komutanlığı", null, null, null },
                    { 77, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Hâkimler ve Savcılar Kurulu", null, null, null },
                    { 78, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Yargıtay Başkanlığı", null, null, null },
                    { 79, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Danıştay Başkanlığı", null, null, null },
                    { 80, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, null, "Türkiye Adalet Akademisi", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "JobListings",
                columns: new[] { "Id", "CategoryKey", "CategoryLabel", "CreatedBy", "CreatedDate", "EndDate", "Institution", "InstitutionId", "IsActive", "IsDeleted", "PdfUrl", "PublishedAt", "SourceUrl", "StartDate", "Summary", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kamu Denetçiliği Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 2, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Rekabet Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 3, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Gelir İdaresi Başkanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 4, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ticaret Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 5, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Arşivleri Başkanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 6, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Adalet Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 7, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Aile ve Sosyal Hizmetler Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 8, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Çevre, Şehircilik ve İklim Değişikliği Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 9, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karayolları Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 10, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tapu ve Kadastro Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 11, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Sermaye Piyasası Kurulu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 12, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Bankacılık Düzenleme ve Denetleme Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 13, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye İstatistik Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 14, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kamu İhale Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 15, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Enerji Piyasaları Düzenleme Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 16, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Gençlik ve Spor Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 17, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Sağlık Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 18, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Aile ve Sosyal Hizmetler Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 19, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tarım ve Orman Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 20, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ulaştırma ve Altyapı Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 21, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Elektrik İletim A.Ş.", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 22, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Eti Maden İşletmeleri", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 23, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Petrolleri A.O.", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 24, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Demiryolları Taşımacılık A.Ş.", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 25, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Boru Hatları ile Petrol Taşıma A.Ş.", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 26, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Sivas Bilim ve Teknoloji Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 27, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Su İşleri Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 28, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "TÜBİTAK", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 29, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karadeniz Teknik Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 30, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kültür ve Turizm Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 31, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ankara Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 32, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ege Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 33, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Marmara Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 34, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Selçuk Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 35, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Bursa Uludağ Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 36, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karabük Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 37, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Hacettepe Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 38, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ondokuz Mayıs Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 39, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Çukurova Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 40, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Erciyes Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 41, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tokat Gaziosmanpaşa Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 42, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Boğaziçi Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 43, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "İstanbul Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 44, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Akdeniz Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 45, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kocaeli Üniversitesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 46, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Taşkömürü Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 47, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Kömür İşletmeleri", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 48, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Orman Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 49, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Malzeme Ofisi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 50, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Makina ve Kimya Endüstrisi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 51, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karayolları Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 52, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Su İşleri Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 53, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Şeker Fabrikaları", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 54, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Toprak Mahsulleri Ofisi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 55, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Belediye İştirakleri Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 56, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tarım İşletmeleri Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 57, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Orman Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 58, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye İstatistik Kurumu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 59, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Demiryolları", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 60, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kıyı Emniyeti Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 61, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Elektrik İletim A.Ş.", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 62, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Posta ve Telgraf Teşkilatı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 63, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Hava Meydanları İşletmesi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 64, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Cumhuriyeti Devlet Demiryolları", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 65, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "İller Bankası", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 66, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karayolları Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 67, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Orman Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 68, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Belediye Başkanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 69, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Su İşleri Genel Müdürlüğü", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 70, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Kömür İşletmeleri", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 71, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Jandarma Genel Komutanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 72, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Milli Savunma Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 73, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kara Kuvvetleri Komutanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 74, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Deniz Kuvvetleri Komutanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 75, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Hava Kuvvetleri Komutanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 76, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Adalet Bakanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 77, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Hâkimler ve Savcılar Kurulu", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 78, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Yargıtay Başkanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 79, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Danıştay Başkanlığı", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 80, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Adalet Akademisi", null, true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionName", "CRUDActionType", "Code", "ControllerName", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "List", 1, "Users_Read", "Users", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcı listeleme ve görüntüleme", true, "Kullanıcı Listele", null, null },
                    { 2, "Add", 2, "Users_Create", "Users", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcı oluşturma", true, "Kullanıcı Oluştur", null, null },
                    { 3, "Update", 3, "Users_Update", "Users", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcı güncelleme", true, "Kullanıcı Güncelle", null, null },
                    { 4, "Delete", 6, "Users_Delete", "Users", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcı silme", true, "Kullanıcı Sil", null, null },
                    { 5, "List", 1, "Roles_Read", "Roles", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol listeleme ve görüntüleme", true, "Rol Listele", null, null },
                    { 6, "Add", 2, "Roles_Create", "Roles", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol oluşturma", true, "Rol Oluştur", null, null },
                    { 7, "Update", 3, "Roles_Update", "Roles", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol güncelleme", true, "Rol Güncelle", null, null },
                    { 8, "Delete", 6, "Roles_Delete", "Roles", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol silme", true, "Rol Sil", null, null },
                    { 9, "List", 1, "Permissions_Read", "Permissions", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "İzin listeleme ve görüntüleme", true, "İzin Listele", null, null },
                    { 10, "Add", 2, "Permissions_Create", "Permissions", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "İzin oluşturma", true, "İzin Oluştur", null, null },
                    { 11, "Update", 3, "Permissions_Update", "Permissions", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "İzin güncelleme", true, "İzin Güncelle", null, null },
                    { 12, "Delete", 6, "Permissions_Delete", "Permissions", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "İzin silme", true, "İzin Sil", null, null },
                    { 13, "List", 1, "Departments_Read", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman listeleme ve görüntüleme", true, "Departman Listele", null, null },
                    { 14, "Add", 2, "Departments_Create", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman oluşturma", true, "Departman Oluştur", null, null },
                    { 15, "Update", 3, "Departments_Update", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman güncelleme", true, "Departman Güncelle", null, null },
                    { 16, "Delete", 6, "Departments_Delete", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman silme", true, "Departman Sil", null, null },
                    { 17, "List", 1, "Menus_Read", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü listeleme ve görüntüleme", true, "Menü Listele", null, null },
                    { 18, "Add", 2, "Menus_Create", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü oluşturma", true, "Menü Oluştur", null, null },
                    { 19, "Update", 3, "Menus_Update", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü güncelleme", true, "Menü Güncelle", null, null },
                    { 20, "Delete", 6, "Menus_Delete", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü silme", true, "Menü Sil", null, null },
                    { 21, "List", 1, "Settings_Read", "Settings", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sistem ayarlarını listeleme", true, "Ayar Listele", null, null },
                    { 22, "Update", 3, "Settings_Update", "Settings", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sistem ayarlarını güncelleme", true, "Ayar Güncelle", null, null },
                    { 23, "List", 1, "AuditLogs_Read", "AuditLogs", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sistem hareketlerini (logları) görüntüleme", true, "Hareket Listele", null, null },
                    { 24, "List", 1, "RecycleBin_Read", "RecycleBin", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Silinmiş kayıtları görüntüleme", true, "Çöp Kutusu Görüntüle", null, null },
                    { 25, "Restore", 3, "RecycleBin_Restore", "RecycleBin", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Silinmiş kayıtları geri yükleme", true, "Çöp Kutusu Geri Yükle", null, null },
                    { 26, "SelectOption", 5, "Roles_SelectOption", "Roles", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol seçim listelerini görüntüleme", true, "Rol Seçenekleri", null, null },
                    { 27, "SelectOption", 5, "Departments_SelectOption", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman seçim listelerini görüntüleme", true, "Departman Seçenekleri", null, null },
                    { 28, "GetStats", 1, "Dashboard_Read", "Dashboard", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dashboard istatistiklerini görüntüleme", true, "Dashboard Görüntüle", null, null },
                    { 29, "AssignRoles", 3, "Users_AssignRoles", "Users", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcılara normal rol atama", true, "Kullanıcı Rolü Ata", null, null },
                    { 30, "AssignPrivilegedRoles", 3, "Users_AssignPrivilegedRoles", "Users", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcılara kritik rol atama", true, "Kritik Kullanıcı Rolü Ata", null, null },
                    { 31, "List", 1, "Sliders_Read", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slaytları listeleme ve görüntüleme", true, "Slider Listele", null, null },
                    { 32, "Add", 2, "Sliders_Create", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slayt oluşturma", true, "Slider Oluştur", null, null },
                    { 33, "Update", 3, "Sliders_Update", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slayt güncelleme", true, "Slider Güncelle", null, null },
                    { 34, "Delete", 6, "Sliders_Delete", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slayt silme", true, "Slider Sil", null, null },
                    { 35, "List", 1, "Jobs_Read", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlanları listeleme ve görüntüleme", true, "İlan Listele", null, null },
                    { 36, "Add", 2, "Jobs_Create", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlan oluşturma", true, "İlan Oluştur", null, null },
                    { 37, "Update", 3, "Jobs_Update", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlan güncelleme", true, "İlan Güncelle", null, null },
                    { 38, "Delete", 6, "Jobs_Delete", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlan silme", true, "İlan Sil", null, null },
                    { 39, "List", 1, "Institutions_Read", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurumları listeleme", true, "Kurum Listele", null, null },
                    { 40, "Add", 2, "Institutions_Create", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurum oluşturma", true, "Kurum Oluştur", null, null },
                    { 41, "Update", 3, "Institutions_Update", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurum güncelleme", true, "Kurum Güncelle", null, null },
                    { 42, "Delete", 6, "Institutions_Delete", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurum silme", true, "Kurum Sil", null, null },
                    { 43, "ResetPassword", 3, "Users_ResetPassword", "Users", null, new DateTime(2026, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kullanıcı parolasını sıfırlama", true, "Kullanıcı Parolası Sıfırla", null, null },
                    { 44, "ResetPrivilegedPassword", 3, "Users_ResetPrivilegedPassword", "Users", null, new DateTime(2026, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ayrıcalıklı hesap parolasını sıfırlama; kullanıcı parolası sıfırlama izni de gerekir", true, "Ayrıcalıklı Hesap Parolası Sıfırla", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "IsPrivileged", "IsSystemRole", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yönetici", true, true, true, "Admin", null, null });

            migrationBuilder.InsertData(
                table: "Sliders",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "ImageUrl", "IsActive", "IsDeleted", "LinkUrl", "Order", "Title", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Üniversite ve bölüm bazında taban puanları ve başarı sıralamalarını karşılaştırarak tercih yap.", "https://images.unsplash.com/photo-1541339907198-e08756dedf3f?w=1200&h=500&fit=crop&auto=format", true, false, "/taban-puanlari/yks", 2, "YKS Tercih Döneminde Doğru Karar", null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Önlisans mezunları için geçiş yapılabilecek bölümler ve güncel taban puanları burada.", "https://images.unsplash.com/photo-1592280771190-3e2e4d571952?w=1200&h=500&fit=crop&auto=format", true, false, "/taban-puanlari/dgs", 3, "DGS ile Lisans Tamamlama Fırsatları", null, null }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate", "IsActive", "Name", "ParentDepartmentId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 2, "BT", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Bilgi Teknolojileri", 1, null, null },
                    { 3, "IK", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "İnsan Kaynakları", 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Icon", "IsActive", "Name", "Order", "ParentId", "PermissionId", "UpdatedBy", "UpdatedDate", "Url" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-speedometer2", true, "Dashboard", 1, null, 28, null, null, "/Admin" },
                    { 2, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-people", true, "Kullanıcılar", 2, null, 1, null, null, "/Admin/Users" },
                    { 3, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-shield-check", true, "Roller", 3, null, 5, null, null, "/Admin/Roles" },
                    { 4, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-key", true, "İzinler", 4, null, 9, null, null, "/Admin/Permissions" },
                    { 5, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-diagram-3", true, "Departmanlar", 5, null, 13, null, null, "/Admin/Departments" },
                    { 6, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-menu-button", true, "Menüler", 6, null, 17, null, null, "/Admin/Menus" },
                    { 7, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-gear", true, "Sistem Ayarları", 7, null, 21, null, null, "/Admin/Settings" },
                    { 8, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-activity", true, "Sistem Hareketleri", 8, null, 23, null, null, "/Admin/AuditLogs" },
                    { 9, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-trash3", true, "Çöp Kutusu", 9, null, 24, null, null, "/Admin/RecycleBin" },
                    { 10, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-megaphone", true, "İlanlar", 10, null, 35, null, null, "/Admin/Jobs" },
                    { 11, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-building", true, "Kurumlar", 11, null, 39, null, null, "/Admin/Institutions" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 },
                    { 21, 1 },
                    { 22, 1 },
                    { 23, 1 },
                    { 24, 1 },
                    { 25, 1 },
                    { 26, 1 },
                    { 27, 1 },
                    { 28, 1 },
                    { 29, 1 },
                    { 30, 1 },
                    { 31, 1 },
                    { 32, 1 },
                    { 33, 1 },
                    { 34, 1 },
                    { 35, 1 },
                    { 36, 1 },
                    { 37, 1 },
                    { 38, 1 },
                    { 39, 1 },
                    { 40, 1 },
                    { 41, 1 },
                    { 42, 1 },
                    { 43, 1 },
                    { 44, 1 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DepartmentId", "Email", "FailedLoginCount", "FirstName", "IsActive", "LastName", "LockoutEndDate", "NormalizedEmail", "NormalizedUsername", "PasswordHash", "Phone", "UpdatedBy", "UpdatedDate", "Username" },
                values: new object[] { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "admin@baselib.com", 0, "Admin", true, "User", null, "ADMIN@BASELIB.COM", "ADMIN", "$2b$10$br5S4nxaGpEKXOPtd/mdvuKBmNoiWHPoJ8MRF43wYnOB/JbBz2o7u", null, null, null, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Code",
                table: "Departments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Name",
                table: "Institutions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_CategoryKey_IsActive_IsDeleted",
                table: "JobListings",
                columns: new[] { "CategoryKey", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_InstitutionId",
                table: "JobListings",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_PermissionId",
                table: "Menus",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_FamilyId",
                table: "RefreshTokens",
                columns: new[] { "UserId", "FamilyId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sliders_IsDeleted_IsActive_Order",
                table: "Sliders",
                columns: new[] { "IsDeleted", "IsActive", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUsername",
                table: "Users",
                column: "NormalizedUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "JobListings");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Institutions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
