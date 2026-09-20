using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "JobListings",
                columns: new[] { "Id", "CategoryKey", "CategoryLabel", "CreatedBy", "CreatedDate", "EndDate", "Institution", "IsActive", "IsDeleted", "PdfUrl", "PublishedAt", "SourceUrl", "StartDate", "Summary", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kamu Denetçiliği Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 2, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Rekabet Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 3, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Gelir İdaresi Başkanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 4, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ticaret Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 5, "memur-a", "A Grubu Memur (Kariyer Meslek)", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Arşivleri Başkanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman yardımcısı alacak", null, null },
                    { 6, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Adalet Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 7, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Aile ve Sosyal Hizmetler Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 8, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Çevre, Şehircilik ve İklim Değişikliği Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 9, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karayolları Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 10, "memur-b", "B Grubu Memur", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tapu ve Kadastro Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Memur alacak", null, null },
                    { 11, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Sermaye Piyasası Kurulu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 12, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Bankacılık Düzenleme ve Denetleme Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 13, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye İstatistik Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 14, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kamu İhale Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 15, "sozlesmeli-kariyer", "Kariyer Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Enerji Piyasaları Düzenleme Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Uzman personel alacak", null, null },
                    { 16, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Gençlik ve Spor Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 17, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Sağlık Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 18, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Aile ve Sosyal Hizmetler Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 19, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tarım ve Orman Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 20, "sozlesmeli-4b", "4/B Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ulaştırma ve Altyapı Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 21, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Elektrik İletim A.Ş.", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 22, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Eti Maden İşletmeleri", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 23, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Petrolleri A.O.", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 24, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Demiryolları Taşımacılık A.Ş.", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 25, "sozlesmeli-kit", "KİT Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Boru Hatları ile Petrol Taşıma A.Ş.", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 26, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Sivas Bilim ve Teknoloji Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 27, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Su İşleri Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 28, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "TÜBİTAK", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 29, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karadeniz Teknik Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 30, "sozlesmeli-kurumsal", "Kurumsal Sözleşmeli Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kültür ve Turizm Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sözleşmeli personel alacak", null, null },
                    { 31, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ankara Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 32, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ege Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 33, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Marmara Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 34, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Selçuk Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 35, "akademik-uye", "Öğretim Üyesi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Bursa Uludağ Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim üyesi alacak", null, null },
                    { 36, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karabük Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 37, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Hacettepe Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 38, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Ondokuz Mayıs Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 39, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Çukurova Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 40, "akademik-gorevli", "Öğretim Görevlisi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Erciyes Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Öğretim görevlisi alacak", null, null },
                    { 41, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tokat Gaziosmanpaşa Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 42, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Boğaziçi Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 43, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "İstanbul Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 44, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Akdeniz Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 45, "akademik-arastirma", "Araştırma Görevlisi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kocaeli Üniversitesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Araştırma görevlisi alacak", null, null },
                    { 46, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Taşkömürü Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 47, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Kömür İşletmeleri", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 48, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Orman Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 49, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Malzeme Ofisi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 50, "isci-kariyer", "Kariyer İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Makina ve Kimya Endüstrisi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Kariyer işçi alacak", null, null },
                    { 51, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karayolları Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 52, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Su İşleri Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 53, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Şeker Fabrikaları", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 54, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Toprak Mahsulleri Ofisi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 55, "isci-surekli", "Sürekli İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Belediye İştirakleri Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Sürekli işçi alacak", null, null },
                    { 56, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Tarım İşletmeleri Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 57, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Orman Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 58, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye İstatistik Kurumu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 59, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Demiryolları", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 60, "isci-gecici", "Geçici İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kıyı Emniyeti Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Geçici işçi alacak", null, null },
                    { 61, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Elektrik İletim A.Ş.", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 62, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Posta ve Telgraf Teşkilatı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 63, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Hava Meydanları İşletmesi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 64, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Cumhuriyeti Devlet Demiryolları", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 65, "isci-engelli", "Engelli İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "İller Bankası", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Engelli işçi alacak", null, null },
                    { 66, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Karayolları Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 67, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Orman Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 68, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Belediye Başkanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 69, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Devlet Su İşleri Genel Müdürlüğü", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 70, "isci-eski-hukumlu", "Eski Hükümlü İşçi", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Kömür İşletmeleri", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Eski hükümlü işçi alacak", null, null },
                    { 71, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Jandarma Genel Komutanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 72, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Milli Savunma Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 73, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Kara Kuvvetleri Komutanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 74, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Deniz Kuvvetleri Komutanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 75, "askeri", "Askeri Personel", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Hava Kuvvetleri Komutanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Personel temin edilecektir", null, null },
                    { 76, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Adalet Bakanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 77, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Hâkimler ve Savcılar Kurulu", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 78, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Yargıtay Başkanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 79, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Danıştay Başkanlığı", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null },
                    { 80, "yargi", "Yargı Mensubu (Hakim - Savcı)", null, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "5 Ekim", "Türkiye Adalet Akademisi", true, false, "https://kamuilan.sbb.gov.tr/", new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://kamuilan.sbb.gov.tr/", "21 Eylül", "Yargı personeli alacak", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionName", "CRUDActionType", "Code", "ControllerName", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 35, "List", 1, "Jobs_Read", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlanları listeleme ve görüntüleme", true, "İlan Listele", null, null },
                    { 36, "Add", 2, "Jobs_Create", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlan oluşturma", true, "İlan Oluştur", null, null },
                    { 37, "Update", 3, "Jobs_Update", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlan güncelleme", true, "İlan Güncelle", null, null },
                    { 38, "Delete", 6, "Jobs_Delete", "Jobs", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İlan silme", true, "İlan Sil", null, null }
                });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Icon", "IsActive", "Name", "Order", "ParentId", "PermissionId", "UpdatedBy", "UpdatedDate", "Url" },
                values: new object[] { 10, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "bi-megaphone", true, "İlanlar", 10, null, 35, null, null, "/Admin/Jobs" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 35, 1 },
                    { 36, 1 },
                    { 37, 1 },
                    { 38, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobListings_CategoryKey_IsActive_IsDeleted",
                table: "JobListings",
                columns: new[] { "CategoryKey", "IsActive", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobListings");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 35, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 36, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 37, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 38, 1 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38);
        }
    }
}
