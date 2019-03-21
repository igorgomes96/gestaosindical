using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class DatabaseEmpresa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Database",
                table: "Empresas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Database",
                table: "Empresas");
        }
    }
}
