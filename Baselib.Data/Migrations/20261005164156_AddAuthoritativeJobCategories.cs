using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthoritativeJobCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentKey = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsSelectable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "JobCategories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsSelectable", "Key", "Label", "ParentKey", "SortOrder", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "memur", "Memur", null, 1, null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "memur-a", "A Grubu Memur (Kariyer Meslek)", "memur", 0, null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "memur-b", "B Grubu Memur", "memur", 1, null, null },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "sozlesmeli", "Sözleşmeli Personel", null, 2, null, null },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", "sozlesmeli", 0, null, null },
                    { 6, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "sozlesmeli-4b", "4/B Sözleşmeli Personel", "sozlesmeli", 1, null, null },
                    { 7, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "sozlesmeli-kit", "KİT Sözleşmeli Personel", "sozlesmeli", 2, null, null },
                    { 8, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", "sozlesmeli", 3, null, null },
                    { 9, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "akademik", "Akademik Personel", null, 3, null, null },
                    { 10, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "akademik-uye", "Öğretim Üyesi", "akademik", 0, null, null },
                    { 11, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "akademik-gorevli", "Öğretim Görevlisi", "akademik", 1, null, null },
                    { 12, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "akademik-arastirma", "Araştırma Görevlisi", "akademik", 2, null, null },
                    { 13, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "isci", "İşçi", null, 4, null, null },
                    { 14, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "isci-kariyer", "Kariyer İşçi", "isci", 0, null, null },
                    { 15, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "isci-surekli", "Sürekli İşçi", "isci", 1, null, null },
                    { 16, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "isci-gecici", "Geçici İşçi", "isci", 2, null, null },
                    { 17, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "isci-engelli", "Engelli İşçi", "isci", 3, null, null },
                    { 18, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "isci-eski-hukumlu", "Eski Hükümlü İşçi", "isci", 4, null, null },
                    { 19, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "askeri", "Askeri Personel", null, 5, null, null },
                    { 20, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, 6, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_Key",
                table: "JobCategories",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobCategories");
        }
    }
}
