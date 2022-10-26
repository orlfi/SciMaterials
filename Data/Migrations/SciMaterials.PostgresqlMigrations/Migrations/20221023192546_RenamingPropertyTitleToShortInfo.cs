using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.PostgresqlMigrations.Migrations
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
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortInfo",
                table: "FileGroups",
                type: "text",
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
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FileGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
