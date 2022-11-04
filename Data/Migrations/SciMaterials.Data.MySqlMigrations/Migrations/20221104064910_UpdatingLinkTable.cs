using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.Data.MySqlMigrations.Migrations
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Links",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Links",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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
