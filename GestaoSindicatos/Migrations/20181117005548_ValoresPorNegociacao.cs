using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class ValoresPorNegociacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContatosSindicatosPatronais_ContatoId",
                table: "ContatosSindicatosPatronais");

            migrationBuilder.DropIndex(
                name: "IX_ContatosSindicatosLaborais_ContatoId",
                table: "ContatosSindicatosLaborais");

            migrationBuilder.DropIndex(
                name: "IX_ContatosEmpresas_ContatoId",
                table: "ContatosEmpresas");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "SindicatosPatronais",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "SindicatosLaborais",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<double>(
                name: "MassaSalarial",
                table: "Negociacoes",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "QtdaTrabalhadores",
                table: "Negociacoes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SindicatosPatronais_Nome",
                table: "SindicatosPatronais",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_SindicatosLaborais_Nome",
                table: "SindicatosLaborais",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_RodadasNegociacoes_Data_NegociacaoId",
                table: "RodadasNegociacoes",
                columns: new[] { "Data", "NegociacaoId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAcao_Data",
                table: "PlanosAcao",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_Negociacoes_Ano_EmpresaId",
                table: "Negociacoes",
                columns: new[] { "Ano", "EmpresaId" },
                unique: true,
                filter: "[EmpresaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Litigios_Data",
                table: "Litigios",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasUsuarios_UserName_EmpresaId",
                table: "EmpresasUsuarios",
                columns: new[] { "UserName", "EmpresaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Nome",
                table: "Empresas",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosPatronais_ContatoId_SindicatoPatronalId",
                table: "ContatosSindicatosPatronais",
                columns: new[] { "ContatoId", "SindicatoPatronalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosLaborais_ContatoId_SindicatoLaboralId",
                table: "ContatosSindicatosLaborais",
                columns: new[] { "ContatoId", "SindicatoLaboralId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContatosEmpresas_ContatoId_EmpresaId",
                table: "ContatosEmpresas",
                columns: new[] { "ContatoId", "EmpresaId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SindicatosPatronais_Nome",
                table: "SindicatosPatronais");

            migrationBuilder.DropIndex(
                name: "IX_SindicatosLaborais_Nome",
                table: "SindicatosLaborais");

            migrationBuilder.DropIndex(
                name: "IX_RodadasNegociacoes_Data_NegociacaoId",
                table: "RodadasNegociacoes");

            migrationBuilder.DropIndex(
                name: "IX_PlanosAcao_Data",
                table: "PlanosAcao");

            migrationBuilder.DropIndex(
                name: "IX_Negociacoes_Ano_EmpresaId",
                table: "Negociacoes");

            migrationBuilder.DropIndex(
                name: "IX_Litigios_Data",
                table: "Litigios");

            migrationBuilder.DropIndex(
                name: "IX_EmpresasUsuarios_UserName_EmpresaId",
                table: "EmpresasUsuarios");

            migrationBuilder.DropIndex(
                name: "IX_Empresas_Nome",
                table: "Empresas");

            migrationBuilder.DropIndex(
                name: "IX_ContatosSindicatosPatronais_ContatoId_SindicatoPatronalId",
                table: "ContatosSindicatosPatronais");

            migrationBuilder.DropIndex(
                name: "IX_ContatosSindicatosLaborais_ContatoId_SindicatoLaboralId",
                table: "ContatosSindicatosLaborais");

            migrationBuilder.DropIndex(
                name: "IX_ContatosEmpresas_ContatoId_EmpresaId",
                table: "ContatosEmpresas");

            migrationBuilder.DropColumn(
                name: "MassaSalarial",
                table: "Negociacoes");

            migrationBuilder.DropColumn(
                name: "QtdaTrabalhadores",
                table: "Negociacoes");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "SindicatosPatronais",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "SindicatosLaborais",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosPatronais_ContatoId",
                table: "ContatosSindicatosPatronais",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosLaborais_ContatoId",
                table: "ContatosSindicatosLaborais",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosEmpresas_ContatoId",
                table: "ContatosEmpresas",
                column: "ContatoId");
        }
    }
}
