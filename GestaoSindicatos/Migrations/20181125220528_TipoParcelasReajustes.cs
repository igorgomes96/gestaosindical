using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class TipoParcelasReajustes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoReajuste",
                table: "ParcelasReajustes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParcelasReajustes_Mes_ReajusteId_TipoReajuste",
                table: "ParcelasReajustes",
                columns: new[] { "Mes", "ReajusteId", "TipoReajuste" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParcelasReajustes_Mes_ReajusteId_TipoReajuste",
                table: "ParcelasReajustes");

            migrationBuilder.DropColumn(
                name: "TipoReajuste",
                table: "ParcelasReajustes");
        }
    }
}
