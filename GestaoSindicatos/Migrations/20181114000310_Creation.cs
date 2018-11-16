using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestaoSindicatos.Migrations
{
    public partial class Creation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contatos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(maxLength: 150, nullable: false),
                    TipoContato = table.Column<int>(nullable: false),
                    Telefone1 = table.Column<string>(maxLength: 30, nullable: true),
                    Telefone2 = table.Column<string>(maxLength: 30, nullable: true),
                    Email = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cidade = table.Column<string>(nullable: false),
                    UF = table.Column<string>(maxLength: 2, nullable: false),
                    Logradouro = table.Column<string>(maxLength: 400, nullable: false),
                    Numero = table.Column<string>(maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reajustes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Salario = table.Column<float>(nullable: false),
                    Piso = table.Column<float>(nullable: false),
                    AuxCreche = table.Column<float>(nullable: false),
                    VaVr = table.Column<float>(nullable: false),
                    VaVrFerias = table.Column<bool>(nullable: false),
                    DescontoVt = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reajustes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SindicatosLaborais",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: false),
                    Cnpj = table.Column<string>(maxLength: 14, nullable: false),
                    Telefone1 = table.Column<string>(maxLength: 30, nullable: true),
                    Telefone2 = table.Column<string>(maxLength: 30, nullable: true),
                    Gestao = table.Column<string>(maxLength: 150, nullable: true),
                    Site = table.Column<string>(maxLength: 150, nullable: true),
                    Federacao = table.Column<string>(maxLength: 150, nullable: true),
                    Cct_act = table.Column<int>(nullable: false),
                    Database = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SindicatosLaborais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SindicatosPatronais",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: false),
                    Cnpj = table.Column<string>(maxLength: 14, nullable: false),
                    Telefone1 = table.Column<string>(maxLength: 30, nullable: true),
                    Telefone2 = table.Column<string>(maxLength: 30, nullable: true),
                    Gestao = table.Column<string>(maxLength: 150, nullable: true),
                    Site = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SindicatosPatronais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 450, nullable: false),
                    Login = table.Column<string>(maxLength: 256, nullable: true),
                    Nome = table.Column<string>(maxLength: 256, nullable: true),
                    Perfil = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContatosSindicatosLaborais",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContatoId = table.Column<int>(nullable: false),
                    SindicatoLaboralId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContatosSindicatosLaborais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContatosSindicatosLaborais_Contatos_ContatoId",
                        column: x => x.ContatoId,
                        principalTable: "Contatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContatosSindicatosLaborais_SindicatosLaborais_SindicatoLaboralId",
                        column: x => x.SindicatoLaboralId,
                        principalTable: "SindicatosLaborais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContatosSindicatosPatronais",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContatoId = table.Column<int>(nullable: false),
                    SindicatoPatronalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContatosSindicatosPatronais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContatosSindicatosPatronais_Contatos_ContatoId",
                        column: x => x.ContatoId,
                        principalTable: "Contatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContatosSindicatosPatronais_SindicatosPatronais_SindicatoPatronalId",
                        column: x => x.SindicatoPatronalId,
                        principalTable: "SindicatosPatronais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cnpj = table.Column<string>(maxLength: 14, nullable: false),
                    Nome = table.Column<string>(maxLength: 200, nullable: false),
                    EnderecoId = table.Column<int>(nullable: false),
                    QtdaTrabalhadores = table.Column<int>(nullable: false),
                    MassaSalarial = table.Column<double>(nullable: false),
                    SindicatoLaboralId = table.Column<int>(nullable: true),
                    SindicatoPatronalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Empresas_SindicatosLaborais_SindicatoLaboralId",
                        column: x => x.SindicatoLaboralId,
                        principalTable: "SindicatosLaborais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Empresas_SindicatosPatronais_SindicatoPatronalId",
                        column: x => x.SindicatoPatronalId,
                        principalTable: "SindicatosPatronais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanosAcao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<string>(maxLength: 2, nullable: false),
                    Referente = table.Column<int>(nullable: false),
                    LaboralId = table.Column<int>(nullable: true),
                    PatronalId = table.Column<int>(nullable: true),
                    Reclamacoes = table.Column<string>(nullable: true),
                    Reclamante = table.Column<string>(maxLength: 256, nullable: true),
                    Procedencia = table.Column<bool>(nullable: false),
                    ResponsavelAcao = table.Column<string>(maxLength: 256, nullable: true),
                    DataSolucao = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanosAcao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanosAcao_SindicatosLaborais_LaboralId",
                        column: x => x.LaboralId,
                        principalTable: "SindicatosLaborais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanosAcao_SindicatosPatronais_PatronalId",
                        column: x => x.PatronalId,
                        principalTable: "SindicatosPatronais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContatosEmpresas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContatoId = table.Column<int>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContatosEmpresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContatosEmpresas_Contatos_ContatoId",
                        column: x => x.ContatoId,
                        principalTable: "Contatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContatosEmpresas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpresasUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmpresaId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresasUsuarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Litigios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmpresaId = table.Column<int>(nullable: false),
                    LaboralId = table.Column<int>(nullable: true),
                    PatronalId = table.Column<int>(nullable: true),
                    Referente = table.Column<int>(nullable: false),
                    Procedimento = table.Column<int>(nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    Assuntos = table.Column<string>(nullable: true),
                    ResumoAssuntos = table.Column<string>(nullable: true),
                    Participantes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Litigios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Litigios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Litigios_SindicatosLaborais_LaboralId",
                        column: x => x.LaboralId,
                        principalTable: "SindicatosLaborais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Litigios_SindicatosPatronais_PatronalId",
                        column: x => x.PatronalId,
                        principalTable: "SindicatosPatronais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Negociacoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ano = table.Column<int>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: true),
                    SindicatoLaboralId = table.Column<int>(nullable: true),
                    SindicatoPatronalId = table.Column<int>(nullable: true),
                    QtdaRodadas = table.Column<int>(nullable: true),
                    TaxaLaboral = table.Column<float>(nullable: true),
                    TaxaPatronal = table.Column<float>(nullable: true),
                    OrcadoId = table.Column<int>(nullable: true),
                    NegociadoId = table.Column<int>(nullable: true),
                    Plr1Sem = table.Column<float>(nullable: true),
                    Plr2Sem = table.Column<float>(nullable: true),
                    StatusACT_CCT = table.Column<int>(nullable: false),
                    StatusPLR = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Negociacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Negociacoes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Negociacoes_Reajustes_NegociadoId",
                        column: x => x.NegociadoId,
                        principalTable: "Reajustes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Negociacoes_Reajustes_OrcadoId",
                        column: x => x.OrcadoId,
                        principalTable: "Reajustes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Negociacoes_SindicatosLaborais_SindicatoLaboralId",
                        column: x => x.SindicatoLaboralId,
                        principalTable: "SindicatosLaborais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Negociacoes_SindicatosPatronais_SindicatoPatronalId",
                        column: x => x.SindicatoPatronalId,
                        principalTable: "SindicatosPatronais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Concorrentes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(maxLength: 200, nullable: true),
                    NegociacaoId = table.Column<int>(nullable: false),
                    ReajusteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concorrentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Concorrentes_Negociacoes_NegociacaoId",
                        column: x => x.NegociacaoId,
                        principalTable: "Negociacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Concorrentes_Reajustes_ReajusteId",
                        column: x => x.ReajusteId,
                        principalTable: "Reajustes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RodadasNegociacoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NegociacaoId = table.Column<int>(nullable: false),
                    Numero = table.Column<int>(nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    Resumo = table.Column<string>(nullable: true),
                    CustosViagens = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RodadasNegociacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RodadasNegociacoes_Negociacoes_NegociacaoId",
                        column: x => x.NegociacaoId,
                        principalTable: "Negociacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Concorrentes_NegociacaoId",
                table: "Concorrentes",
                column: "NegociacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Concorrentes_ReajusteId",
                table: "Concorrentes",
                column: "ReajusteId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosEmpresas_ContatoId",
                table: "ContatosEmpresas",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosEmpresas_EmpresaId",
                table: "ContatosEmpresas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosLaborais_ContatoId",
                table: "ContatosSindicatosLaborais",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosLaborais_SindicatoLaboralId",
                table: "ContatosSindicatosLaborais",
                column: "SindicatoLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosPatronais_ContatoId",
                table: "ContatosSindicatosPatronais",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContatosSindicatosPatronais_SindicatoPatronalId",
                table: "ContatosSindicatosPatronais",
                column: "SindicatoPatronalId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_EnderecoId",
                table: "Empresas",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_SindicatoLaboralId",
                table: "Empresas",
                column: "SindicatoLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_SindicatoPatronalId",
                table: "Empresas",
                column: "SindicatoPatronalId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasUsuarios_EmpresaId",
                table: "EmpresasUsuarios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Litigios_EmpresaId",
                table: "Litigios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Litigios_LaboralId",
                table: "Litigios",
                column: "LaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_Litigios_PatronalId",
                table: "Litigios",
                column: "PatronalId");

            migrationBuilder.CreateIndex(
                name: "IX_Negociacoes_EmpresaId",
                table: "Negociacoes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Negociacoes_NegociadoId",
                table: "Negociacoes",
                column: "NegociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Negociacoes_OrcadoId",
                table: "Negociacoes",
                column: "OrcadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Negociacoes_SindicatoLaboralId",
                table: "Negociacoes",
                column: "SindicatoLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_Negociacoes_SindicatoPatronalId",
                table: "Negociacoes",
                column: "SindicatoPatronalId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAcao_LaboralId",
                table: "PlanosAcao",
                column: "LaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAcao_PatronalId",
                table: "PlanosAcao",
                column: "PatronalId");

            migrationBuilder.CreateIndex(
                name: "IX_RodadasNegociacoes_NegociacaoId",
                table: "RodadasNegociacoes",
                column: "NegociacaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Concorrentes");

            migrationBuilder.DropTable(
                name: "ContatosEmpresas");

            migrationBuilder.DropTable(
                name: "ContatosSindicatosLaborais");

            migrationBuilder.DropTable(
                name: "ContatosSindicatosPatronais");

            migrationBuilder.DropTable(
                name: "EmpresasUsuarios");

            migrationBuilder.DropTable(
                name: "Litigios");

            migrationBuilder.DropTable(
                name: "PlanosAcao");

            migrationBuilder.DropTable(
                name: "RodadasNegociacoes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Contatos");

            migrationBuilder.DropTable(
                name: "Negociacoes");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Reajustes");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropTable(
                name: "SindicatosLaborais");

            migrationBuilder.DropTable(
                name: "SindicatosPatronais");
        }
    }
}
