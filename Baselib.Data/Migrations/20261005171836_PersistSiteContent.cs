using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class PersistSiteContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobCategories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subject = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamCountdowns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Target = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCountdowns", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InformationPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Slug = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Body = table.Column<string>(type: "varchar(8000)", maxLength: 8000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationPages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PublicComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Body = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Page = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    HideName = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicComments_PublicComments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PublicComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScoreCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScorePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScorePeriods", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SiteAdvertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Position = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkUrl = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAdvertisements", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SiteNavigation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteNavigation", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SiteNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Section = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Link = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteNews", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudyLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyLevels", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudyPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyLevelId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPrograms_StudyLevels_StudyLevelId",
                        column: x => x.StudyLevelId,
                        principalTable: "StudyLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ScoreEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScoreCategoryId = table.Column<int>(type: "int", nullable: false),
                    StudyProgramId = table.Column<int>(type: "int", nullable: false),
                    ScorePeriodId = table.Column<int>(type: "int", nullable: false),
                    Institution = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quota = table.Column<int>(type: "int", nullable: false),
                    Vacant = table.Column<int>(type: "int", nullable: false),
                    MinScore = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(10,5)", precision: 10, scale: 5, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Qualification = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreEntries_ScoreCategories_ScoreCategoryId",
                        column: x => x.ScoreCategoryId,
                        principalTable: "ScoreCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoreEntries_ScorePeriods_ScorePeriodId",
                        column: x => x.ScorePeriodId,
                        principalTable: "ScorePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScoreEntries_StudyPrograms_StudyProgramId",
                        column: x => x.StudyProgramId,
                        principalTable: "StudyPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "ContactMessages",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "IsActive", "IsDeleted", "Message", "Name", "Status", "Subject", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "elif@example.com", true, false, "Merhaba, KPSS puan hesaplamasında...", "Elif Şahin", "Yeni", "Puan hesaplama hakkında", null, null },
                    { 2, null, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "burak@example.com", true, false, "Sitenizde reklam vermek istiyorum.", "Burak Demir", "Okundu", "Reklam iş birliği", null, null }
                });

            migrationBuilder.InsertData(
                table: "ExamCountdowns",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Label", "SortOrder", "Target", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "YKS", 0, new DateTime(2026, 6, 20, 10, 15, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "KPSS Ortaöğretim", 1, new DateTime(2026, 9, 13, 10, 15, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "KPSS Önlisans", 2, new DateTime(2026, 9, 6, 10, 15, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "KPSS Lisans", 3, new DateTime(2026, 7, 19, 10, 15, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "DGS", 4, new DateTime(2026, 7, 5, 10, 15, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 6, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "ALES", 5, new DateTime(2026, 5, 10, 10, 15, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.InsertData(
                table: "InformationPages",
                columns: new[] { "Id", "Body", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Slug", "Title", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "hakkimizda", "Hakkımızda", null, null },
                    { 2, "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "gizlilik", "Gizlilik Politikası", null, null },
                    { 3, "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "kullanim-kosullari", "Kullanım Koşulları", null, null },
                    { 4, "Sınavlar, taban puanları ve kamu personel alım ilanlarını bir arada inceleyebileceğiniz bir platformdur.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "ales", "ALES", null, null }
                });

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 16,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 17,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 18,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 19,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "JobCategories",
                keyColumn: "Id",
                keyValue: 20,
                column: "IsDeleted",
                value: false);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionName", "CRUDActionType", "Code", "ControllerName", "CreatedBy", "CreatedDate", "Description", "IsActive", "IsDeleted", "IsSystem", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 45, "Read", 1, "Content_Read", "Content", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, true, "Site İçeriği Görüntüle", null, null },
                    { 46, "Create", 2, "Content_Create", "Content", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, true, "Site İçeriği Oluştur", null, null },
                    { 47, "Update", 3, "Content_Update", "Content", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, true, "Site İçeriği Güncelle", null, null },
                    { 48, "Delete", 6, "Content_Delete", "Content", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, true, "Site İçeriği Sil", null, null }
                });

            migrationBuilder.InsertData(
                table: "PublicComments",
                columns: new[] { "Id", "Author", "Body", "CreatedBy", "CreatedDate", "Email", "HideName", "IsActive", "IsDeleted", "Likes", "Page", "ParentId", "Status", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "Ahmet Y.", "KPSS lisans için bu puanlar yeterli mi? Bilgisayar mühendisliği düşünüyorum.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 3, "/taban-puanlari/kpss-lisans/bolum/5", null, "Onaylı", null, null },
                    { 5, "Zeynep D.", "Sağlık Bakanlığı kadrolarında taban puan biraz daha düşük görünüyor.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 4, "/taban-puanlari/kpss-lisans/bolum/5", null, "Onaylı", null, null },
                    { 6, "Ahmet Y.", "KPSS lisans için bu puanlar yeterli mi?", null, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 0, "/taban-puanlari/kpss-lisans/bolum/5", null, "Onaylı", null, null },
                    { 7, "Zeynep D.", "Sağlık Bakanlığı kadrolarında taban puan daha düşük görünüyor.", null, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 0, "/taban-puanlari/kpss-lisans/bolum/5", null, "Beklemede", null, null },
                    { 8, "guest_4821", "ucuz takipçi -> bit.ly/xxx", null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 0, "/taban-puanlari/kpss-lisans/bolum/5", null, "Spam", null, null }
                });

            migrationBuilder.InsertData(
                table: "ScoreCategories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Key", "Label", "SortOrder", "Title", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "kpss-lisans", "Taban Puanı", 0, "KPSS Lisans", null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "kpss-onlisans", "Taban Puanı", 1, "KPSS Önlisans", null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "kpss-orta", "Taban Puanı", 2, "KPSS Ortaöğretim", null, null },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "yks", "Taban Puanı", 3, "YKS", null, null },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "dgs", "Taban Puanı", 4, "DGS", null, null }
                });

            migrationBuilder.InsertData(
                table: "ScorePeriods",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Name", "SortOrder", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2024/2", 0, null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2024/1", 1, null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2023/2", 2, null, null },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2023/1", 3, null, null },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2024", 4, null, null }
                });

            migrationBuilder.InsertData(
                table: "SiteAdvertisements",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "ImageUrl", "IsActive", "IsDeleted", "LinkUrl", "Name", "Position", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://images.unsplash.com/photo-1557804506-669a67965ba0?w=400&h=250&fit=crop", true, false, "https://reklam.example.com", "Anasayfa Üst Banner", "home", null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "", true, false, "https://reklam.example.com", "Sidebar Reklamı", "sidebar", null, null }
                });

            migrationBuilder.InsertData(
                table: "SiteNavigation",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Label", "SortOrder", "UpdatedBy", "UpdatedDate", "Url" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Ana Sayfa", 0, null, null, "/" },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "İlanlar", 1, null, null, "/ilanlar" },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "KPSS", 2, null, null, "/taban-puanlari/kpss-lisans" },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "YKS", 3, null, null, "/taban-puanlari/yks" },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "ALES", 4, null, null, "/bilgi/ales" },
                    { 6, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "DGS", 5, null, null, "/taban-puanlari/dgs" },
                    { 7, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "İletişim", 6, null, null, "/iletisim" }
                });

            migrationBuilder.InsertData(
                table: "SiteNews",
                columns: new[] { "Id", "Category", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Link", "PublishedAt", "Section", "SortOrder", "Title", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/kpss-lisans", new DateTime(2024, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 0, "KPSS 2024/2 Atama Taban Puanları", null, null },
                    { 2, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/kpss-lisans", new DateTime(2024, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 1, "Bilgisayar Mühendisliği YKS Sıralamaları", null, null },
                    { 3, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/kpss-lisans", new DateTime(2024, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 2, "Adalet Öğretmenliği Atama Puanları", null, null },
                    { 4, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/kpss-lisans", new DateTime(2024, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 3, "DGS Hemşirelik Geçiş Puanları", null, null },
                    { 5, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "https://www.osym.gov.tr", new DateTime(2024, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "osym", 0, "2024 KPSS tercih kılavuzu yayımlandı", null, null },
                    { 6, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "https://www.osym.gov.tr", new DateTime(2024, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "osym", 1, "ALES/3 başvuruları başladı", null, null },
                    { 7, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "https://www.osym.gov.tr", new DateTime(2024, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "osym", 2, "YKS ek yerleştirme takvimi açıklandı", null, null },
                    { 8, "YKS", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/yks/bolum/5", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 1, "Bilgisayar Mühendisliği 2024 Taban Puanı", null, null },
                    { 9, "KPSS", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/kpss-onlisans", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 2, "KPSS Önlisans Atama Puanları", null, null },
                    { 10, "DGS", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "/taban-puanlari/dgs/bolum/7", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "recent", 3, "Hemşirelik DGS Geçiş Puanları", null, null },
                    { 11, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "https://osym.gov.tr", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "osym", 0, "2026 YKS Başvuru Kılavuzu Yayımlandı", null, null },
                    { 12, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "https://osym.gov.tr", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "osym", 0, "KPSS 2026 Sınav Takvimi Açıklandı", null, null },
                    { 13, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "https://osym.gov.tr", new DateTime(2026, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "osym", 0, "DGS Tercih İşlemleri Başladı", null, null }
                });

            migrationBuilder.InsertData(
                table: "StudyLevels",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Key", "Label", "SortOrder", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "lisans", "Lisans", 0, null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "onlisans", "Önlisans", 1, null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "ortaogretim", "Ortaöğretim", 2, null, null }
                });

            migrationBuilder.InsertData(
                table: "PublicComments",
                columns: new[] { "Id", "Author", "Body", "CreatedBy", "CreatedDate", "Email", "HideName", "IsActive", "IsDeleted", "Likes", "Page", "ParentId", "Status", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 2, "Ayşe K.", "Geçen dönem bu puanla atananlar oldu, ama kadro sayısına da bakmak lazım.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 1, "/taban-puanlari/kpss-lisans/bolum/5", 1, "Onaylı", null, null },
                    { 4, "Mehmet T.", "Kurum tercihine göre değişir. Ankara dışını da işaretlersen şansın artar.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 2, "/taban-puanlari/kpss-lisans/bolum/5", 1, "Onaylı", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 45, 1 },
                    { 46, 1 },
                    { 47, 1 },
                    { 48, 1 }
                });

            migrationBuilder.InsertData(
                table: "StudyPrograms",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "Name", "StudyLevelId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Acil Yardım ve Afet Yönetimi", 1, null, null },
                    { 2, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Adalet Öğretmenliği", 1, null, null },
                    { 3, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Aktüerya", 1, null, null },
                    { 4, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Alman Dili ve Edebiyatı", 1, null, null },
                    { 5, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Bilgisayar Mühendisliği", 1, null, null },
                    { 6, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Elektrik-Elektronik Mühendisliği", 1, null, null },
                    { 7, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Hemşirelik", 1, null, null },
                    { 8, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Hukuk", 1, null, null },
                    { 9, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "İşletme", 1, null, null },
                    { 10, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Psikoloji", 1, null, null },
                    { 11, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Adalet", 2, null, null },
                    { 12, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Bilgisayar Programcılığı", 2, null, null },
                    { 13, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Büro Yönetimi ve Yönetici Asistanlığı", 2, null, null },
                    { 14, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "İlk ve Acil Yardım", 2, null, null },
                    { 15, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Muhasebe ve Vergi Uygulamaları", 2, null, null },
                    { 16, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Tıbbi Dokümantasyon ve Sekreterlik", 2, null, null },
                    { 17, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Büro Memurluğu", 3, null, null },
                    { 18, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Veri Hazırlama ve Kontrol İşletmeni", 3, null, null },
                    { 19, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Zabıt Katipliği", 3, null, null },
                    { 20, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Hizmetli", 3, null, null },
                    { 21, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Büro Yönetimi", 2, null, null },
                    { 22, null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "Tıp", 1, null, null }
                });

            migrationBuilder.InsertData(
                table: "PublicComments",
                columns: new[] { "Id", "Author", "Body", "CreatedBy", "CreatedDate", "Email", "HideName", "IsActive", "IsDeleted", "Likes", "Page", "ParentId", "Status", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 3, "Ahmet Y.", "Teşekkürler, kadro sayısına bakayım.", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, true, false, 0, "/taban-puanlari/kpss-lisans/bolum/5", 2, "Onaylı", null, null });

            migrationBuilder.InsertData(
                table: "ScoreEntries",
                columns: new[] { "Id", "City", "CreatedBy", "CreatedDate", "Institution", "IsActive", "IsDeleted", "MaxScore", "MinScore", "Qualification", "Quota", "Rank", "ScoreCategoryId", "ScorePeriodId", "StudyProgramId", "Title", "UpdatedBy", "UpdatedDate", "Vacant" },
                values: new object[,]
                {
                    { 1, "Ankara", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adalet Bakanlığı", true, false, 89.21876m, 80.95434m, "3607 - Bilgisayar Müh.", 12, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 2, "İstanbul", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sağlık Bakanlığı", true, false, 81.21216m, 80.99622m, "3607 - Bilgisayar Müh.", 8, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 3, "İzmir", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Milli Eğitim Bakanlığı", true, false, 84.7719m, 78.4512m, "3607 - Bilgisayar Müh.", 5, 0, 1, 1, 5, "Mühendis", null, null, 1 },
                    { 4, "Bursa", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Çevre, Şehircilik ve İklim Değişikliği Bakanlığı", true, false, 83.4102m, 79.1033m, "3607 - Bilgisayar Müh.", 6, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 5, "Ankara", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hazine ve Maliye Bakanlığı", true, false, 90.0021m, 82.331m, "3607 - Bilgisayar Müh.", 10, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 6, "Antalya", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ulaştırma ve Altyapı Bakanlığı", true, false, 82.1109m, 76.8801m, "3607 - Bilgisayar Müh.", 4, 0, 1, 1, 5, "Mühendis", null, null, 2 },
                    { 7, "Konya", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gençlik ve Spor Bakanlığı", true, false, 80.9931m, 77.5522m, "3607 - Bilgisayar Müh.", 3, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 8, "Samsun", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tarım ve Orman Bakanlığı", true, false, 81.4407m, 75.2201m, "3607 - Bilgisayar Müh.", 7, 0, 1, 1, 5, "Mühendis", null, null, 1 },
                    { 9, "Ankara", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İçişleri Bakanlığı", true, false, 91.2298m, 83.7712m, "3607 - Bilgisayar Müh.", 9, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 10, "Kocaeli", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sanayi ve Teknoloji Bakanlığı", true, false, 85.5533m, 79.9012m, "3607 - Bilgisayar Müh.", 5, 0, 1, 1, 5, "Mühendis", null, null, 0 },
                    { 11, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sağlık Bakanlığı", true, false, 88.42m, 88.42m, "", 0, 1240, 2, 5, 12, "", null, null, 0 },
                    { 12, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adalet Bakanlığı", true, false, 82.15m, 82.15m, "", 0, 3980, 2, 5, 21, "", null, null, 0 },
                    { 13, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "İTÜ", true, false, 342.8m, 342.8m, "", 0, 210, 5, 5, 5, "", null, null, 0 },
                    { 14, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ege Üniversitesi", true, false, 318.5m, 318.5m, "", 0, 640, 5, 5, 7, "", null, null, 0 },
                    { 15, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hacettepe Üniversitesi", true, false, 541.2m, 541.2m, "", 0, 850, 4, 5, 22, "", null, null, 0 },
                    { 16, "", null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ankara Üniversitesi", true, false, 498.6m, 498.6m, "", 0, 5400, 4, 5, 8, "", null, null, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InformationPages_Slug",
                table: "InformationPages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicComments_Page_Status",
                table: "PublicComments",
                columns: new[] { "Page", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicComments_ParentId",
                table: "PublicComments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategories_Key",
                table: "ScoreCategories",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntries_ScoreCategoryId_StudyProgramId_ScorePeriodId",
                table: "ScoreEntries",
                columns: new[] { "ScoreCategoryId", "StudyProgramId", "ScorePeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntries_ScorePeriodId",
                table: "ScoreEntries",
                column: "ScorePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEntries_StudyProgramId",
                table: "ScoreEntries",
                column: "StudyProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ScorePeriods_Name",
                table: "ScorePeriods",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyLevels_Key",
                table: "StudyLevels",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyPrograms_StudyLevelId",
                table: "StudyPrograms",
                column: "StudyLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "ExamCountdowns");

            migrationBuilder.DropTable(
                name: "InformationPages");

            migrationBuilder.DropTable(
                name: "PublicComments");

            migrationBuilder.DropTable(
                name: "ScoreEntries");

            migrationBuilder.DropTable(
                name: "SiteAdvertisements");

            migrationBuilder.DropTable(
                name: "SiteNavigation");

            migrationBuilder.DropTable(
                name: "SiteNews");

            migrationBuilder.DropTable(
                name: "ScoreCategories");

            migrationBuilder.DropTable(
                name: "ScorePeriods");

            migrationBuilder.DropTable(
                name: "StudyPrograms");

            migrationBuilder.DropTable(
                name: "StudyLevels");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 45, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 46, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 47, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 48, 1 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobCategories");
        }
    }
}
