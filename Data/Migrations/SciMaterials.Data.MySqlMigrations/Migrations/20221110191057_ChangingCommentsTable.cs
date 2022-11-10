using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.Data.MySqlMigrations.Migrations
{
    public partial class ChangingCommentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_FileGroups_FileGroupId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Files_FileId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Urls_UrlId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Resources_ResourceId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Comments_FileGroupId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_FileId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UrlId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "FileGroupId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UrlId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "ResourceId",
                table: "Ratings",
                newName: "UrlId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_ResourceId",
                table: "Ratings",
                newName: "IX_Ratings_UrlId");

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Comments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ResourceId",
                table: "Comments",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Resources_ResourceId",
                table: "Comments",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Urls_UrlId",
                table: "Ratings",
                column: "UrlId",
                principalTable: "Urls",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Resources_ResourceId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Urls_UrlId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ResourceId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "UrlId",
                table: "Ratings",
                newName: "ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UrlId",
                table: "Ratings",
                newName: "IX_Ratings_ResourceId");

            migrationBuilder.AddColumn<Guid>(
                name: "FileGroupId",
                table: "Comments",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "Comments",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UrlId",
                table: "Comments",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FileGroupId",
                table: "Comments",
                column: "FileGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FileId",
                table: "Comments",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UrlId",
                table: "Comments",
                column: "UrlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_FileGroups_FileGroupId",
                table: "Comments",
                column: "FileGroupId",
                principalTable: "FileGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Files_FileId",
                table: "Comments",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Urls_UrlId",
                table: "Comments",
                column: "UrlId",
                principalTable: "Urls",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Resources_ResourceId",
                table: "Ratings",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
