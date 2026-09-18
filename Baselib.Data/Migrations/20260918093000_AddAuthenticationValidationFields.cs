using System;
using Baselib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baselib.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260918093000_AddAuthenticationValidationFields")]
public partial class AddAuthenticationValidationFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "FailedLoginCount",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "LockoutEndDate",
            table: "Users",
            type: "datetime(6)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NormalizedEmail",
            table: "Users",
            type: "varchar(254)",
            maxLength: 254,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NormalizedUsername",
            table: "Users",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.Sql("""
            UPDATE `Users`
            SET `NormalizedUsername` = UPPER(TRIM(`Username`)),
                `NormalizedEmail` = UPPER(TRIM(`Email`))
            """);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedEmail",
            table: "Users",
            type: "varchar(254)",
            maxLength: 254,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(254)",
            oldMaxLength: 254,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NormalizedUsername",
            table: "Users",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100,
            oldNullable: true);

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
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Users_NormalizedEmail", table: "Users");
        migrationBuilder.DropIndex(name: "IX_Users_NormalizedUsername", table: "Users");
        migrationBuilder.DropColumn(name: "FailedLoginCount", table: "Users");
        migrationBuilder.DropColumn(name: "LockoutEndDate", table: "Users");
        migrationBuilder.DropColumn(name: "NormalizedEmail", table: "Users");
        migrationBuilder.DropColumn(name: "NormalizedUsername", table: "Users");
    }
}
