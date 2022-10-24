using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.SQLiteMigrations.Migrations
{
    public partial class RenamingPropertyTitleToShortInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "FileGroups");

            migrationBuilder.AddColumn<string>(
                name: "ShortInfo",
                table: "Files",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortInfo",
                table: "FileGroups",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortInfo",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ShortInfo",
                table: "FileGroups");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FileGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
