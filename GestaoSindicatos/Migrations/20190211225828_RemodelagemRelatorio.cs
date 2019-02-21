using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class RemodelagemRelatorio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RespostasRelatorio_Negociacoes_NegociacaoId",
                table: "RespostasRelatorio");

            migrationBuilder.DropTable(
                name: "Pergunta");

            migrationBuilder.DropColumn(
                name: "GrupoPergunta",
                table: "RespostasRelatorio");

            migrationBuilder.DropColumn(
                name: "OrdemGrupo",
                table: "RespostasRelatorio");

            migrationBuilder.RenameColumn(
                name: "NegociacaoId",
                table: "RespostasRelatorio",
                newName: "GrupoPerguntaId");

            migrationBuilder.RenameIndex(
                name: "IX_RespostasRelatorio_NegociacaoId",
                table: "RespostasRelatorio",
                newName: "IX_RespostasRelatorio_GrupoPerguntaId");

            migrationBuilder.AddColumn<int>(
                name: "RelatorioId",
                table: "GruposPerguntas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GruposPerguntasPadrao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ordem = table.Column<int>(nullable: false),
                    Texto = table.Column<string>(maxLength: 400, nullable: false),
                    Default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposPerguntasPadrao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NegociacaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relatorios_Negociacoes_NegociacaoId",
                        column: x => x.NegociacaoId,
                        principalTable: "Negociacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerguntasPadrao",
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
                    table.PrimaryKey("PK_PerguntasPadrao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerguntasPadrao_GruposPerguntasPadrao_GrupoPerguntaId",
                        column: x => x.GrupoPerguntaId,
                        principalTable: "GruposPerguntasPadrao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GruposPerguntas_RelatorioId",
                table: "GruposPerguntas",
                column: "RelatorioId");

            migrationBuilder.CreateIndex(
                name: "IX_PerguntasPadrao_GrupoPerguntaId",
                table: "PerguntasPadrao",
                column: "GrupoPerguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_NegociacaoId",
                table: "Relatorios",
                column: "NegociacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_GruposPerguntas_Relatorios_RelatorioId",
                table: "GruposPerguntas",
                column: "RelatorioId",
                principalTable: "Relatorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RespostasRelatorio_GruposPerguntas_GrupoPerguntaId",
                table: "RespostasRelatorio",
                column: "GrupoPerguntaId",
                principalTable: "GruposPerguntas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GruposPerguntas_Relatorios_RelatorioId",
                table: "GruposPerguntas");

            migrationBuilder.DropForeignKey(
                name: "FK_RespostasRelatorio_GruposPerguntas_GrupoPerguntaId",
                table: "RespostasRelatorio");

            migrationBuilder.DropTable(
                name: "PerguntasPadrao");

            migrationBuilder.DropTable(
                name: "Relatorios");

            migrationBuilder.DropTable(
                name: "GruposPerguntasPadrao");

            migrationBuilder.DropIndex(
                name: "IX_GruposPerguntas_RelatorioId",
                table: "GruposPerguntas");

            migrationBuilder.DropColumn(
                name: "RelatorioId",
                table: "GruposPerguntas");

            migrationBuilder.RenameColumn(
                name: "GrupoPerguntaId",
                table: "RespostasRelatorio",
                newName: "NegociacaoId");

            migrationBuilder.RenameIndex(
                name: "IX_RespostasRelatorio_GrupoPerguntaId",
                table: "RespostasRelatorio",
                newName: "IX_RespostasRelatorio_NegociacaoId");

            migrationBuilder.AddColumn<string>(
                name: "GrupoPergunta",
                table: "RespostasRelatorio",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrdemGrupo",
                table: "RespostasRelatorio",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pergunta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GrupoPerguntaId = table.Column<int>(nullable: true),
                    Ordem = table.Column<int>(nullable: false),
                    Texto = table.Column<string>(maxLength: 1000, nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_RespostasRelatorio_Negociacoes_NegociacaoId",
                table: "RespostasRelatorio",
                column: "NegociacaoId",
                principalTable: "Negociacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
