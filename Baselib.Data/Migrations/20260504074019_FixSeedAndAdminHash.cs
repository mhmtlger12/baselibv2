using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Baselib.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedAndAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionName", "CRUDActionType", "Code", "ControllerName", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 12, "Delete", 8, "Permissions_Delete", "Permissions", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "İzin silme", true, "İzin Sil", null, null },
                    { 13, "List", 2, "Departments_Read", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman listeleme ve görüntüleme", true, "Departman Listele", null, null },
                    { 14, "Add", 1, "Departments_Create", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman oluşturma", true, "Departman Oluştur", null, null },
                    { 15, "Update", 4, "Departments_Update", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman güncelleme", true, "Departman Güncelle", null, null },
                    { 16, "Delete", 8, "Departments_Delete", "Departments", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Departman silme", true, "Departman Sil", null, null },
                    { 17, "List", 2, "Menus_Read", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü listeleme ve görüntüleme", true, "Menü Listele", null, null },
                    { 18, "Add", 1, "Menus_Create", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü oluşturma", true, "Menü Oluştur", null, null },
                    { 19, "Update", 4, "Menus_Update", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü güncelleme", true, "Menü Güncelle", null, null },
                    { 20, "Delete", 8, "Menus_Delete", "Menus", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Menü silme", true, "Menü Sil", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 2,
                column: "PermissionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 3,
                column: "PermissionId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 4,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 5,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 6,
                column: "PermissionId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "Description", "Name" },
                values: new object[] { "List", 2, "Users_Read", "Kullanıcı listeleme ve görüntüleme", "Kullanıcı Listele" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ActionName", "Description" },
                values: new object[] { "Add", "Kullanıcı oluşturma" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Kullanıcı güncelleme");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Kullanıcı silme");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "Description", "Name" },
                values: new object[] { "List", 2, "Roles_Read", "Rol listeleme ve görüntüleme", "Rol Listele" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ActionName", "Description" },
                values: new object[] { "Add", "Rol oluşturma" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "Rol güncelleme");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "Rol silme");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "Description", "Name" },
                values: new object[] { "List", 2, "Permissions_Read", "İzin listeleme ve görüntüleme", "İzin Listele" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "ControllerName", "Description", "Name" },
                values: new object[] { "Add", 1, "Permissions_Create", "Permissions", "İzin oluşturma", "İzin Oluştur" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "ControllerName", "Description", "Name" },
                values: new object[] { "Update", 4, "Permissions_Update", "Permissions", "İzin güncelleme", "İzin Güncelle" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2b$10$br5S4nxaGpEKXOPtd/mdvuKBmNoiWHPoJ8MRF43wYnOB/JbBz2o7u");

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 16, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 17, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 18, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 19, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 20, 1 });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 2,
                column: "PermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 3,
                column: "PermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 4,
                column: "PermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 5,
                column: "PermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 6,
                column: "PermissionId",
                value: null);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "Description", "Name" },
                values: new object[] { "Index", 15, "Users_Index", "Kullanıcı Listesi", "Kullanıcı Listesi" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ActionName", "Description" },
                values: new object[] { "Create", "Kullanıcı Oluştur" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Kullanıcı Güncelle");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Kullanıcı Sil");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "Description", "Name" },
                values: new object[] { "Index", 15, "Roles_Index", "Rol Listesi", "Rol Listesi" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ActionName", "Description" },
                values: new object[] { "Create", "Rol Oluştur" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "Rol Güncelle");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "Rol Sil");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "Description", "Name" },
                values: new object[] { "Index", 15, "Permissions_Index", "İzin Listesi", "İzin Listesi" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "ControllerName", "Description", "Name" },
                values: new object[] { "Index", 15, "Departments_Index", "Departments", "Departman Listesi", "Departman Listesi" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ActionName", "CRUDActionType", "Code", "ControllerName", "Description", "Name" },
                values: new object[] { "Index", 15, "Menus_Index", "Menus", "Menü Listesi", "Menü Listesi" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$10$N9qo8uLOickgx2ZMRZoMye4kDgR8KD6CWJ5C3u0s0p0s0s0s0s0s0u");
        }
    }
}
