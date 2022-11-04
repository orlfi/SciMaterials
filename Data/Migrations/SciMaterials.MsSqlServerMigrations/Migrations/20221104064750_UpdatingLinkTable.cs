using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.MsSqlServerMigrations.Migrations
{
    public partial class UpdatingLinkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Authors_AuthorId",
                table: "Links");

            migrationBuilder.DropIndex(
                name: "IX_Links_AuthorId",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Links");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Links",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Links",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Links",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Links",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Links_AuthorId",
                table: "Links",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Authors_AuthorId",
                table: "Links",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
