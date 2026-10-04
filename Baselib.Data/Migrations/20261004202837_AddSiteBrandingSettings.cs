using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteBrandingSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "Key", "UpdatedBy", "UpdatedDate", "Value" },
                values: new object[,]
                {
                    { 3, null, new DateTime(2026, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Site adı (en fazla 100 karakter)", true, "SiteName", null, null, "puannokta" },
                    { 4, null, new DateTime(2026, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Site sloganı (en fazla 200 karakter)", true, "SiteTagline", null, null, "Taban puanları, tek noktada." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
