using Baselib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baselib.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260918100000_RemoveUnusedMaintenanceModeSetting")]
public partial class RemoveUnusedMaintenanceModeSetting : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "AppSettings", keyColumn: "Id", keyValue: 3);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "AppSettings",
            columns: new[] { "Id", "Key", "Value", "Description", "IsActive", "CreatedDate" },
            values: new object[] { 3, "MaintenanceMode", "false", "Sistemi bakım moduna alır", true, new DateTime(2025, 1, 1) });
    }
}
