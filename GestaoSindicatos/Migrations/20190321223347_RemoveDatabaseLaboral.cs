using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class RemoveDatabaseLaboral : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Database",
                table: "SindicatosLaborais");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Database",
                table: "SindicatosLaborais",
                nullable: false,
                defaultValue: 0);
        }
    }
}
