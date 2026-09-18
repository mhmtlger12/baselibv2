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
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Uygulamanın genel adı", true, "SiteName", null, null, "Baselib" },
                    { 2, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maksimum hatalı giriş denemesi", true, "MaxLoginAttempts", null, null, "5" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate", "IsActive", "Name", "ParentDepartmentId", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 1, "YT", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Yönetim", null, null, null });

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
                    { 28, "GetStats", 1, "Dashboard_Read", "Dashboard", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dashboard istatistiklerini görüntüleme", true, "Dashboard Görüntüle", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yönetici", true, "Admin", null, null });

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
                    { 9, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-trash3", true, "Çöp Kutusu", 9, null, 24, null, null, "/Admin/RecycleBin" }
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
                    { 28, 1 }
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
                name: "Menus");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

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
