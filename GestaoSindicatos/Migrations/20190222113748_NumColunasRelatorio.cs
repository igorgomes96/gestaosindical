using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class NumColunasRelatorio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumColunas",
                table: "RespostasRelatorio",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumColunas",
                table: "PerguntasPadrao",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumColunas",
                table: "RespostasRelatorio");

            migrationBuilder.DropColumn(
                name: "NumColunas",
                table: "PerguntasPadrao");
        }
    }
}
