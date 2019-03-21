﻿// <auto-generated />
using System;
using GestaoSindicatos.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GestaoSindicatos.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GestaoSindicatos.Model.Arquivo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContentType")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("DataUpload");

                    b.Property<int>("DependencyId");

                    b.Property<int>("DependencyType");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Path")
                        .HasMaxLength(255);

                    b.Property<long>("Tamanho");

                    b.HasKey("Id");

                    b.HasIndex("DependencyId", "DependencyType");

                    b.ToTable("Arquivos");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Concorrente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("NegociacaoId");

                    b.Property<string>("Nome")
                        .HasMaxLength(200);

                    b.Property<int>("ReajusteId");

                    b.HasKey("Id");

                    b.HasIndex("NegociacaoId");

                    b.HasIndex("ReajusteId");

                    b.ToTable("Concorrentes");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Contato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasMaxLength(150);

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<string>("Telefone1")
                        .HasMaxLength(30);

                    b.Property<string>("Telefone2")
                        .HasMaxLength(30);

                    b.Property<int>("TipoContato");

                    b.HasKey("Id");

                    b.ToTable("Contatos");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ContatoEmpresa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContatoId");

                    b.Property<int>("EmpresaId");

                    b.HasKey("Id");

                    b.HasIndex("EmpresaId");

                    b.HasIndex("ContatoId", "EmpresaId")
                        .IsUnique();

                    b.ToTable("ContatosEmpresas");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ContatoSindicatoLaboral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContatoId");

                    b.Property<int>("SindicatoLaboralId");

                    b.HasKey("Id");

                    b.HasIndex("SindicatoLaboralId");

                    b.HasIndex("ContatoId", "SindicatoLaboralId")
                        .IsUnique();

                    b.ToTable("ContatosSindicatosLaborais");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ContatoSindicatoPatronal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContatoId");

                    b.Property<int>("SindicatoPatronalId");

                    b.HasKey("Id");

                    b.HasIndex("SindicatoPatronalId");

                    b.HasIndex("ContatoId", "SindicatoPatronalId")
                        .IsUnique();

                    b.ToTable("ContatosSindicatosPatronais");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Empresa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14);

                    b.Property<int>("Database");

                    b.Property<int>("EnderecoId");

                    b.Property<double>("MassaSalarial");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("QtdaTrabalhadores");

                    b.Property<int?>("SindicatoLaboralId");

                    b.Property<int?>("SindicatoPatronalId");

                    b.HasKey("Id");

                    b.HasIndex("EnderecoId");

                    b.HasIndex("Nome");

                    b.HasIndex("SindicatoLaboralId");

                    b.HasIndex("SindicatoPatronalId");

                    b.ToTable("Empresas");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.EmpresaUsuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmpresaId");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("EmpresaId");

                    b.HasIndex("UserName", "EmpresaId")
                        .IsUnique();

                    b.ToTable("EmpresasUsuarios");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Endereco", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Bairro")
                        .HasMaxLength(150);

                    b.Property<string>("Cidade")
                        .IsRequired();

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasMaxLength(400);

                    b.Property<string>("Numero")
                        .HasMaxLength(6);

                    b.Property<string>("UF")
                        .IsRequired()
                        .HasMaxLength(2);

                    b.HasKey("Id");

                    b.ToTable("Enderecos");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.GrupoPergunta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Ordem");

                    b.Property<int>("RelatorioId");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasMaxLength(400);

                    b.HasKey("Id");

                    b.HasIndex("RelatorioId");

                    b.ToTable("GruposPerguntas");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.GrupoPerguntaPadrao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Ordem");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasMaxLength(400);

                    b.HasKey("Id");

                    b.ToTable("GruposPerguntasPadrao");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ItemLitigio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Assuntos");

                    b.Property<int>("LitigioId");

                    b.Property<int>("PlanoAcaoId");

                    b.Property<bool>("PossuiPlano");

                    b.HasKey("Id");

                    b.HasIndex("LitigioId");

                    b.ToTable("ItensLitigios");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Litigio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Assuntos");

                    b.Property<DateTime>("Data");

                    b.Property<int>("EmpresaId");

                    b.Property<string>("Estado")
                        .HasMaxLength(2);

                    b.Property<int?>("LaboralId");

                    b.Property<string>("Participantes");

                    b.Property<int?>("PatronalId");

                    b.Property<int>("Procedimento");

                    b.Property<int>("Referente");

                    b.Property<string>("ResumoAssuntos");

                    b.HasKey("Id");

                    b.HasIndex("Data");

                    b.HasIndex("EmpresaId");

                    b.HasIndex("LaboralId");

                    b.HasIndex("PatronalId");

                    b.ToTable("Litigios");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Negociacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Ano");

                    b.Property<int?>("EmpresaId");

                    b.Property<double>("MassaSalarial");

                    b.Property<int?>("NegociadoId");

                    b.Property<int?>("OrcadoId");

                    b.Property<float?>("Plr1Sem");

                    b.Property<float?>("Plr2Sem");

                    b.Property<int?>("QtdaRodadas");

                    b.Property<int>("QtdaTrabalhadores");

                    b.Property<int?>("SindicatoLaboralId");

                    b.Property<int?>("SindicatoPatronalId");

                    b.Property<int>("StatusACT_CCT");

                    b.Property<int>("StatusPLR");

                    b.Property<float?>("TaxaLaboral");

                    b.Property<float?>("TaxaPatronal");

                    b.HasKey("Id");

                    b.HasIndex("EmpresaId");

                    b.HasIndex("NegociadoId");

                    b.HasIndex("OrcadoId");

                    b.HasIndex("SindicatoLaboralId");

                    b.HasIndex("SindicatoPatronalId");

                    b.HasIndex("Ano", "EmpresaId")
                        .IsUnique()
                        .HasFilter("[EmpresaId] IS NOT NULL");

                    b.ToTable("Negociacoes");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ParcelaReajuste", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Mes");

                    b.Property<int>("ReajusteId");

                    b.Property<int>("TipoReajuste");

                    b.Property<float>("Valor");

                    b.HasKey("Id");

                    b.HasIndex("ReajusteId");

                    b.HasIndex("Mes", "ReajusteId", "TipoReajuste")
                        .IsUnique();

                    b.ToTable("ParcelasReajustes");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.PerguntaPadrao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GrupoPerguntaId");

                    b.Property<int>("NumColunas");

                    b.Property<int>("Ordem");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("GrupoPerguntaId");

                    b.ToTable("PerguntasPadrao");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.PlanoAcao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Data");

                    b.Property<DateTime>("DataPrevista");

                    b.Property<DateTime?>("DataSolucao");

                    b.Property<string>("Descricao");

                    b.Property<int>("ItemLitigioId");

                    b.Property<bool>("Procedencia");

                    b.Property<string>("ResponsavelAcao")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("Data");

                    b.HasIndex("ItemLitigioId")
                        .IsUnique();

                    b.ToTable("PlanosAcao");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Reajuste", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("AuxCreche");

                    b.Property<float>("DescontoVt");

                    b.Property<float>("Piso");

                    b.Property<float>("Salario");

                    b.Property<float>("VaVr");

                    b.Property<float>("VaVrFerias");

                    b.HasKey("Id");

                    b.ToTable("Reajustes");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Relatorio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("NegociacaoId");

                    b.Property<string>("Titulo")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("NegociacaoId");

                    b.ToTable("Relatorios");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.RespostaRelatorio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AplicacaoResposta");

                    b.Property<int>("GrupoPerguntaId");

                    b.Property<int>("NumColunas");

                    b.Property<int>("Ordem");

                    b.Property<string>("Pergunta")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<string>("Resposta")
                        .HasMaxLength(4000);

                    b.HasKey("Id");

                    b.HasIndex("GrupoPerguntaId");

                    b.ToTable("RespostasRelatorio");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.RodadaNegociacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float?>("CustosViagens");

                    b.Property<DateTime>("Data");

                    b.Property<int>("NegociacaoId");

                    b.Property<int>("Numero");

                    b.Property<string>("Resumo");

                    b.HasKey("Id");

                    b.HasIndex("NegociacaoId");

                    b.HasIndex("Data", "NegociacaoId");

                    b.ToTable("RodadasNegociacoes");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.SindicatoLaboral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Cct_act");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14);

                    b.Property<int>("Database");

                    b.Property<string>("Federacao")
                        .HasMaxLength(150);

                    b.Property<string>("Gestao")
                        .HasMaxLength(150);

                    b.Property<string>("Nome")
                        .IsRequired();

                    b.Property<string>("Site")
                        .HasMaxLength(150);

                    b.Property<string>("Telefone1")
                        .HasMaxLength(30);

                    b.Property<string>("Telefone2")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("Nome");

                    b.ToTable("SindicatosLaborais");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.SindicatoPatronal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14);

                    b.Property<string>("Gestao")
                        .HasMaxLength(150);

                    b.Property<string>("Nome")
                        .IsRequired();

                    b.Property<string>("Site")
                        .HasMaxLength(150);

                    b.Property<string>("Telefone1")
                        .HasMaxLength(30);

                    b.Property<string>("Telefone2")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("Nome");

                    b.ToTable("SindicatosPatronais");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Usuario", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450);

                    b.Property<string>("Login")
                        .HasMaxLength(256);

                    b.Property<string>("Nome")
                        .HasMaxLength(256);

                    b.Property<string>("Perfil")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Concorrente", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Negociacao", "Negociacao")
                        .WithMany("Concorrentes")
                        .HasForeignKey("NegociacaoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GestaoSindicatos.Model.Reajuste", "Reajuste")
                        .WithMany()
                        .HasForeignKey("ReajusteId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ContatoEmpresa", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Contato", "Contato")
                        .WithMany()
                        .HasForeignKey("ContatoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GestaoSindicatos.Model.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ContatoSindicatoLaboral", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Contato", "Contato")
                        .WithMany()
                        .HasForeignKey("ContatoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GestaoSindicatos.Model.SindicatoLaboral", "SindicatoLaboral")
                        .WithMany()
                        .HasForeignKey("SindicatoLaboralId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ContatoSindicatoPatronal", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Contato", "Contato")
                        .WithMany()
                        .HasForeignKey("ContatoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GestaoSindicatos.Model.SindicatoPatronal", "SindicatoPatronal")
                        .WithMany()
                        .HasForeignKey("SindicatoPatronalId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Empresa", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Endereco", "Endereco")
                        .WithMany()
                        .HasForeignKey("EnderecoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GestaoSindicatos.Model.SindicatoLaboral", "SindicatoLaboral")
                        .WithMany()
                        .HasForeignKey("SindicatoLaboralId");

                    b.HasOne("GestaoSindicatos.Model.SindicatoPatronal", "SindicatoPatronal")
                        .WithMany()
                        .HasForeignKey("SindicatoPatronalId");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.EmpresaUsuario", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.GrupoPergunta", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Relatorio", "Relatorio")
                        .WithMany("GruposPerguntas")
                        .HasForeignKey("RelatorioId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ItemLitigio", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Litigio", "Litigio")
                        .WithMany("Itens")
                        .HasForeignKey("LitigioId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Litigio", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GestaoSindicatos.Model.SindicatoLaboral", "Laboral")
                        .WithMany()
                        .HasForeignKey("LaboralId");

                    b.HasOne("GestaoSindicatos.Model.SindicatoPatronal", "Patronal")
                        .WithMany()
                        .HasForeignKey("PatronalId");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Negociacao", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaId");

                    b.HasOne("GestaoSindicatos.Model.Reajuste", "Negociado")
                        .WithMany()
                        .HasForeignKey("NegociadoId");

                    b.HasOne("GestaoSindicatos.Model.Reajuste", "Orcado")
                        .WithMany()
                        .HasForeignKey("OrcadoId");

                    b.HasOne("GestaoSindicatos.Model.SindicatoLaboral", "SindicatoLaboral")
                        .WithMany()
                        .HasForeignKey("SindicatoLaboralId");

                    b.HasOne("GestaoSindicatos.Model.SindicatoPatronal", "SindicatoPatronal")
                        .WithMany()
                        .HasForeignKey("SindicatoPatronalId");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.ParcelaReajuste", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Reajuste", "Reajuste")
                        .WithMany("Parcelas")
                        .HasForeignKey("ReajusteId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.PerguntaPadrao", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.GrupoPerguntaPadrao", "GrupoPergunta")
                        .WithMany("Perguntas")
                        .HasForeignKey("GrupoPerguntaId");
                });

            modelBuilder.Entity("GestaoSindicatos.Model.PlanoAcao", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.ItemLitigio", "ItemLitigio")
                        .WithOne("PlanoAcao")
                        .HasForeignKey("GestaoSindicatos.Model.PlanoAcao", "ItemLitigioId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.Relatorio", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Negociacao", "Negociacao")
                        .WithMany()
                        .HasForeignKey("NegociacaoId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.RespostaRelatorio", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.GrupoPergunta", "GrupoPergunta")
                        .WithMany("Respostas")
                        .HasForeignKey("GrupoPerguntaId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("GestaoSindicatos.Model.RodadaNegociacao", b =>
                {
                    b.HasOne("GestaoSindicatos.Model.Negociacao", "Negociacao")
                        .WithMany("RodadasNegociacoes")
                        .HasForeignKey("NegociacaoId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
