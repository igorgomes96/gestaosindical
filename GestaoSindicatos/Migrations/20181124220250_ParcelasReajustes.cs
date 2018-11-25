using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class ParcelasReajustes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParcelasReajustes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Mes = table.Column<int>(nullable: false),
                    Valor = table.Column<float>(nullable: false),
                    ReajusteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelasReajustes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcelasReajustes_Reajustes_ReajusteId",
                        column: x => x.ReajusteId,
                        principalTable: "Reajustes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParcelasReajustes_ReajusteId",
                table: "ParcelasReajustes",
                column: "ReajusteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParcelasReajustes");
        }
    }
}
