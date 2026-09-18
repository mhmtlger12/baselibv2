using System;
using Baselib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baselib.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260918090000_HardenRefreshTokenStorage")]
public partial class HardenRefreshTokenStorage : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ActiveRoleId",
            table: "RefreshTokens",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FamilyId",
            table: "RefreshTokens",
            type: "varchar(32)",
            maxLength: 32,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUsedDate",
            table: "RefreshTokens",
            type: "datetime(6)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RevokedDate",
            table: "RefreshTokens",
            type: "datetime(6)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RevokedReason",
            table: "RefreshTokens",
            type: "varchar(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TokenHash",
            table: "RefreshTokens",
            type: "varchar(64)",
            maxLength: 64,
            nullable: true);

        // Mevcut geçerli oturumlar korunur; raw değer aynı transaction içinde SHA-256 özetiyle değiştirilir.
        migrationBuilder.Sql("""
            UPDATE `RefreshTokens`
            SET `TokenHash` = SHA2(`Token`, 256),
                `FamilyId` = REPLACE(UUID(), '-', ''),
                `IpAddress` = LEFT(`IpAddress`, 45),
                `UserAgent` = LEFT(`UserAgent`, 512)
            """);

        migrationBuilder.AlterColumn<string>(
            name: "TokenHash",
            table: "RefreshTokens",
            type: "varchar(64)",
            maxLength: 64,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(64)",
            oldMaxLength: 64,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FamilyId",
            table: "RefreshTokens",
            type: "varchar(32)",
            maxLength: 32,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(32)",
            oldMaxLength: 32,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "IpAddress",
            table: "RefreshTokens",
            type: "varchar(45)",
            maxLength: 45,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserAgent",
            table: "RefreshTokens",
            type: "varchar(512)",
            maxLength: 512,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext",
            oldNullable: true);

        migrationBuilder.DropColumn(
            name: "Token",
            table: "RefreshTokens");

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_TokenHash",
            table: "RefreshTokens",
            column: "TokenHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_UserId_FamilyId",
            table: "RefreshTokens",
            columns: new[] { "UserId", "FamilyId" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException(
            "This security migration is intentionally irreversible because raw refresh tokens cannot be recovered from their SHA-256 hashes.");
    }
}
