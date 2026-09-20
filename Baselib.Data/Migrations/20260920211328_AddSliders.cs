using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSliders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionName", "CRUDActionType", "Code", "ControllerName", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 31, "List", 1, "Sliders_Read", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slaytları listeleme ve görüntüleme", true, "Slider Listele", null, null },
                    { 32, "Add", 2, "Sliders_Create", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slayt oluşturma", true, "Slider Oluştur", null, null },
                    { 33, "Update", 3, "Sliders_Update", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slayt güncelleme", true, "Slider Güncelle", null, null },
                    { 34, "Delete", 6, "Sliders_Delete", "Sliders", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Slayt silme", true, "Slider Sil", null, null }
                });

            migrationBuilder.InsertData(
                table: "Sliders",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "ImageUrl", "IsActive", "IsDeleted", "LinkUrl", "Order", "Title", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Üniversite ve bölüm bazında taban puanları ve başarı sıralamalarını karşılaştırarak tercih yap.", "https://images.unsplash.com/photo-1541339907198-e08756dedf3f?w=1200&h=500&fit=crop&auto=format", true, false, "/taban-puanlari/yks", 2, "YKS Tercih Döneminde Doğru Karar", null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Önlisans mezunları için geçiş yapılabilecek bölümler ve güncel taban puanları burada.", "https://images.unsplash.com/photo-1592280771190-3e2e4d571952?w=1200&h=500&fit=crop&auto=format", true, false, "/taban-puanlari/dgs", 3, "DGS ile Lisans Tamamlama Fırsatları", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 31, 1 },
                    { 32, 1 },
                    { 33, 1 },
                    { 34, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sliders_IsDeleted_IsActive_Order",
                table: "Sliders",
                columns: new[] { "IsDeleted", "IsActive", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 31, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 32, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 33, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 34, 1 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34);
        }
    }
}
