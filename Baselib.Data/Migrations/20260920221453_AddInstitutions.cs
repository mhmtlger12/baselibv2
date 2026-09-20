using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "JobListings",
                type: "int",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 1,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 2,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 3,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 4,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 5,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 6,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 7,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 8,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 9,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 10,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 11,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 12,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 13,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 14,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 15,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 16,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 17,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 18,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 19,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 20,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 21,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 22,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 23,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 24,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 25,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 26,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 27,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 28,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 29,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 30,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 31,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 32,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 33,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 34,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 35,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 36,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 37,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 38,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 39,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 40,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 41,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 42,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 43,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 44,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 45,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 46,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 47,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 48,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 49,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 50,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 51,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 52,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 53,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 54,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 55,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 56,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 57,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 58,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 59,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 60,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 61,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 62,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 63,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 64,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 65,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 66,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 67,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 68,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 69,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 70,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 71,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 72,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 73,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 74,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 75,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 76,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 77,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 78,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 79,
                column: "InstitutionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobListings",
                keyColumn: "Id",
                keyValue: 80,
                column: "InstitutionId",
                value: null);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionName", "CRUDActionType", "Code", "ControllerName", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 39, "List", 1, "Institutions_Read", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurumları listeleme", true, "Kurum Listele", null, null },
                    { 40, "Add", 2, "Institutions_Create", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurum oluşturma", true, "Kurum Oluştur", null, null },
                    { 41, "Update", 3, "Institutions_Update", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurum güncelleme", true, "Kurum Güncelle", null, null },
                    { 42, "Delete", 6, "Institutions_Delete", "Institutions", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kurum silme", true, "Kurum Sil", null, null }
                });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Icon", "IsActive", "Name", "Order", "ParentId", "PermissionId", "UpdatedBy", "UpdatedDate", "Url" },
                values: new object[] { 11, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-building", true, "Kurumlar", 11, null, 39, null, null, "/Admin/Institutions" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 39, 1 },
                    { 40, 1 },
                    { 41, 1 },
                    { 42, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_InstitutionId",
                table: "JobListings",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Name",
                table: "Institutions",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_Institutions_InstitutionId",
                table: "JobListings",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_Institutions_InstitutionId",
                table: "JobListings");

            migrationBuilder.DropTable(
                name: "Institutions");

            migrationBuilder.DropIndex(
                name: "IX_JobListings_InstitutionId",
                table: "JobListings");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 39, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 40, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 41, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 42, 1 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "JobListings");
        }
    }
}
