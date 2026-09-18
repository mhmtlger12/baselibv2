using System;
using Baselib.Core.Enums;
using Baselib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baselib.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260918103000_AddDashboardReadPermission")]
public partial class AddDashboardReadPermission : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Permissions",
            columns: new[] { "Id", "Name", "ControllerName", "ActionName", "Code", "Description", "CRUDActionType", "IsActive", "CreatedDate" },
            values: new object[] { 28, "Dashboard Görüntüle", "Dashboard", "GetStats", "Dashboard_Read", "Dashboard istatistiklerini görüntüleme", (int)CRUDActionType.View, true, new DateTime(2025, 1, 1) });

        migrationBuilder.InsertData(
            table: "RolePermissions",
            columns: new[] { "RoleId", "PermissionId" },
            values: new object[] { 1, 28 });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "RoleId", "PermissionId" }, keyValues: new object[] { 1, 28 });
        migrationBuilder.DeleteData(table: "Permissions", keyColumn: "Id", keyValue: 28);
    }
}
