using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciMaterials.PostgresqlMigrations.Migrations
{
    public partial class Add_ResourceType_To_Resources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResourceType",
                table: "Resources",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "Resources");
        }
    }
}
