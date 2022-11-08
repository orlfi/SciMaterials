using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.SQLiteMigrations.Migrations
{
    public partial class ChangeFilesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AntivirusScanDate",
                table: "Files",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AntivirusScanStatus",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AntivirusScanDate",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AntivirusScanStatus",
                table: "Files");
        }
    }
}
