using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class Relatorio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GruposPerguntas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ordem = table.Column<int>(nullable: false),
                    Texto = table.Column<string>(maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposPerguntas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RespostasRelatorio",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ordem = table.Column<int>(nullable: false),
                    Pergunta = table.Column<string>(maxLength: 1000, nullable: false),
                    Resposta = table.Column<string>(maxLength: 4000, nullable: true),
                    GrupoPergunta = table.Column<string>(maxLength: 400, nullable: true),
                    OrdemGrupo = table.Column<int>(nullable: true),
                    NegociacaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasRelatorio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespostasRelatorio_Negociacoes_NegociacaoId",
                        column: x => x.NegociacaoId,
                        principalTable: "Negociacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pergunta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ordem = table.Column<int>(nullable: false),
                    Texto = table.Column<string>(maxLength: 1000, nullable: false),
                    GrupoPerguntaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pergunta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pergunta_GruposPerguntas_GrupoPerguntaId",
                        column: x => x.GrupoPerguntaId,
                        principalTable: "GruposPerguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pergunta_GrupoPerguntaId",
                table: "Pergunta",
                column: "GrupoPerguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasRelatorio_NegociacaoId",
                table: "RespostasRelatorio",
                column: "NegociacaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pergunta");

            migrationBuilder.DropTable(
                name: "RespostasRelatorio");

            migrationBuilder.DropTable(
                name: "GruposPerguntas");
        }
    }
}
