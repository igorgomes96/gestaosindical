using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GestaoSindicatos
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<Context>(opt =>
            //{
            //    opt.UseInMemoryDatabase("GestaoSindical");
            //});

            services.AddDbContext<Context>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Base"));
            });

            services.AddScoped(typeof(CrudService<>));
            services.AddTransient<ArquivosRepository>();
            services.AddTransient<ArquivosService>();
            services.AddScoped<SindicatosLaboraisService>();
            services.AddScoped<SindicatosPatronaisService>();
            services.AddScoped<EmpresasService>();
            services.AddScoped<ContatosService>();
            services.AddScoped<NegociacoesService>();
            services.AddScoped<RodadasService>();
            services.AddScoped<LitigiosService>();
            services.AddScoped<PlanosAcaoService>();
            services.AddScoped<ConcorrentesService>();
            services.AddScoped<UsuariosService>();
            services.AddScoped<PesquisaService>();
            services.AddScoped<DashboardService>();
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


            // Authentication
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Base")));

            // Ativando a utilização do ASP.NET Identity, a fim de
            // permitir a recuperação de seus objetos via injeção de
            // dependências
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthContext>()
                .AddDefaultTokenProviders();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;

            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Context context, ArquivosRepository arquivosRepository,
            AuthContext authContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // SeedDataTest(context, arquivosRepository);

            // Criação de estruturas, usuários e permissões
            // na base do ASP.NET Identity Core (caso ainda não
            // existam)
            new IdentityInitializer(authContext, userManager, roleManager).Initialize();

            app.UseCors("AllowAll");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        private void SeedDataTest(Context context, ArquivosRepository arquivosRepository)
        {
            Contato contato1 = new Contato
            {
                Nome = "Contato 1",
                Email = "contato1@email.com",
                Telefone1 = "(11) 1111-1111",
                Telefone2 = "(22) 1111-1111",
                TipoContato = TipoContato.Presidente
            };
            Contato contato2 = new Contato
            {
                Nome = "Contato 2",
                Email = "contato2@email.com",
                Telefone1 = "(11) 2222-2222",
                Telefone2 = "(22) 2222-2222",
                TipoContato = TipoContato.Negociador
            };
            Contato contato3 = new Contato
            {
                Nome = "Contato 3",
                Email = "contato3@email.com",
                Telefone1 = "(11) 3333-3333",
                Telefone2 = "(22) 3333-3333",
                TipoContato = TipoContato.Contato
            };
            Contato contato4 = new Contato
            {
                Nome = "Contato 4",
                Email = "contato4@email.com",
                Telefone1 = "(11) 4444-4444",
                Telefone2 = "(22) 4444-4444",
                TipoContato = TipoContato.Outro
            };

            SindicatoLaboral laboral1 = new SindicatoLaboral
            {
                Nome = "Laboral 1",
                Cnpj = "11111111111111",
                Cct_act = CCT_ACT.ACT,
                Database = Mes.Marco,
                Federacao = "Federação 1",
                Gestao = "Gestão 1",
                Site = "laboral.com.br",
                Telefone1 = "1111111111"
            };
            SindicatoLaboral laboral2 = new SindicatoLaboral
            {
                Nome = "Laboral 2",
                Cnpj = "22222222222222",
                Cct_act = CCT_ACT.ACT,
                Database = Mes.Janeiro,
                Federacao = "Federação 2",
                Gestao = "Gestão 2",
                Site = "laboral.com.br",
                Telefone2 = "1122222222"
            };

            SindicatoPatronal patronal1 = new SindicatoPatronal
            {
                Nome = "Patronal 1",
                Cnpj = "11111111111111",
                Gestao = "Gestão 1",
                Site = "patronal.com.br",
                Telefone1 = "1111111111"
            };

            SindicatoPatronal patronal2 = new SindicatoPatronal
            {
                Nome = "Patronal 2",
                Cnpj = "22222222222222",
                Gestao = "Gestão 2",
                Site = "patronal.com.br",
                Telefone2 = "1122222222"
            };

            Endereco endereco1 = new Endereco
            {
                Cidade = "Cidade 1",
                Logradouro = "Logradouro 1",
                Numero = "1",
                UF = "BA"
            };

            Endereco endereco2 = new Endereco
            {
                Cidade = "Cidade 2",
                Logradouro = "Logradouro 1",
                Numero = "1",
                UF = "AC"
            };

            Empresa empresa1 = new Empresa
            {
                Cnpj = "11111111222222",
                Nome = "Empresa 1",
                MassaSalarial = 100000,
                QtdaTrabalhadores = 100,
                SindicatoLaboralId = 1,
                SindicatoPatronalId = 1
            };

            Empresa empresa2 = new Empresa
            {
                Cnpj = "11111111333333",
                Nome = "Empresa 2",
                MassaSalarial = 200000,
                QtdaTrabalhadores = 200,
                SindicatoLaboralId = 2,
                SindicatoPatronalId = 2
            };

            context.Contatos.Add(contato1);
            context.Contatos.Add(contato2);
            context.Contatos.Add(contato3);
            context.Contatos.Add(contato4);

            context.SindicatosLaborais.Add(laboral1);
            context.SindicatosLaborais.Add(laboral2);

            context.SindicatosPatronais.Add(patronal1);
            context.SindicatosPatronais.Add(patronal2);

            context.Enderecos.Add(endereco1);
            context.Enderecos.Add(endereco2);

            empresa1.EnderecoId = endereco1.Id;
            empresa2.EnderecoId = endereco2.Id;
            context.Empresas.Add(empresa1);
            context.Empresas.Add(empresa2);

            Negociacao negociacao1 = new Negociacao
            {
                Ano = 2018,
                EmpresaId = empresa1.Id,
                SindicatoLaboralId = laboral1.Id,
                SindicatoPatronalId = patronal1.Id,
                QtdaRodadas = 1,
                StatusACT_CCT = StatusNegociacao.NaoIniciada,
                StatusPLR = StatusNegociacao.Dissidio,
                TaxaLaboral = 3746,
                TaxaPatronal = 1827,
                Plr1Sem = 2000,
                Plr2Sem = 3872,
                Orcado = new Reajuste
                {
                    Salario = 0.1F,
                    Piso = 0.1F,
                    AuxCreche = 0.1F,
                    DescontoVt = 0.06F,
                    VaVr = 0.1F,
                    VaVrFerias = true
                },
                Negociado = new Reajuste
                {
                    Salario = 0.09F,
                    Piso = 0.09F,
                    AuxCreche = 0.09F,
                    DescontoVt = 0.06F,
                    VaVr = 0.09F,
                    VaVrFerias = true
                }
            };

            Negociacao negociacao2 = new Negociacao
            {
                Ano = 2018,
                EmpresaId = empresa2.Id,
                SindicatoLaboralId = laboral2.Id,
                SindicatoPatronalId = patronal2.Id,
                QtdaRodadas = 1,
                StatusACT_CCT = StatusNegociacao.EmNegociacao,
                StatusPLR = StatusNegociacao.Fechada,
                TaxaLaboral = 1000,
                TaxaPatronal = 2222,
                Plr1Sem = 2000,
                Plr2Sem = 3872,
                Orcado = new Reajuste
                {
                    Salario = 0.1F,
                    Piso = 0.1F,
                    AuxCreche = 0.1F,
                    DescontoVt = 0.06F,
                    VaVr = 0.1F,
                    VaVrFerias = true
                },
                Negociado = new Reajuste
                {
                    Salario = 0.09F,
                    Piso = 0.09F,
                    AuxCreche = 0.09F,
                    DescontoVt = 0.06F,
                    VaVr = 0.09F,
                    VaVrFerias = true
                }
            };

            RodadaNegociacao rodada1 = new RodadaNegociacao
            {
                Data = new DateTime(2018, 10, 1),
                Numero = 1,
                Resumo = "Teste Resumo 1",
                CustosViagens = 4000
            };

            RodadaNegociacao rodada2 = new RodadaNegociacao
            {
                Data = new DateTime(2018, 10, 2),
                Numero = 1,
                Resumo = "Teste Resumo 2",
                CustosViagens = 5000
            };

            RodadaNegociacao rodada3 = new RodadaNegociacao
            {
                Data = new DateTime(2018, 9, 10),
                Numero = 2,
                Resumo = "Teste Resumo 3",
                CustosViagens = 6000
            };

            RodadaNegociacao rodada4 = new RodadaNegociacao
            {
                Data = new DateTime(2018, 10, 10),
                Numero = 2,
                Resumo = "Teste Resumo 4",
                CustosViagens = 7000
            };

            Litigio litigio1 = new Litigio
            {
                Assuntos = "Assuntos 1",
                Data = new DateTime(2018, 1, 1),
                EmpresaId = empresa1.Id,
                Referente = Referente.MPT,
                Participantes = "Participante 1, Participante 2",
                Procedimento = ProcedimentoLitigio.Audiencia,
                ResumoAssuntos = "Resumo Teste 1"
            };

            Litigio litigio2 = new Litigio
            {
                Assuntos = "Assuntos 2",
                Data = new DateTime(2018, 5, 10),
                EmpresaId = empresa2.Id,
                Referente = Referente.Laboral,
                LaboralId = laboral2.Id,
                Participantes = "Participante 1, Participante 2",
                Procedimento = ProcedimentoLitigio.MesaRedonda,
                ResumoAssuntos = "Resumo Teste 2"
            };

            PlanoAcao plano1 = new PlanoAcao
            {
                Data = new DateTime(2018, 4, 2),
                Estado = "MG",
                Procedencia = false,
                Reclamacoes = "Todas as Reclamações",
                Reclamante = "Reclamante 1",
                ResponsavelAcao = "Responsável 1",
                Referente = Referente.Patronal,
                PatronalId = patronal1.Id
            };

            PlanoAcao plano2 = new PlanoAcao
            {
                Data = new DateTime(2018, 4, 2),
                Estado = "MG",
                Procedencia = false,
                Reclamacoes = "Todas as Reclamações",
                Reclamante = "Reclamante 1",
                ResponsavelAcao = "Responsável 1",
                Referente = Referente.MTE,
                DataSolucao = new DateTime(2018, 6, 1)
            };

            context.Litigios.Add(litigio1);
            context.Litigios.Add(litigio2);

            context.Negociacoes.Add(negociacao1);
            context.Negociacoes.Add(negociacao2);
            rodada1.NegociacaoId = negociacao1.Id;
            rodada2.NegociacaoId = negociacao2.Id;
            rodada3.NegociacaoId = negociacao1.Id;
            rodada4.NegociacaoId = negociacao2.Id;
            context.RodadasNegociacoes.Add(rodada1);
            context.RodadasNegociacoes.Add(rodada2);
            context.RodadasNegociacoes.Add(rodada3);
            context.RodadasNegociacoes.Add(rodada4);
            context.PlanosAcao.Add(plano1);
            context.PlanosAcao.Add(plano2);

            context.Concorrentes.Add(new Concorrente
            {
                NegociacaoId = negociacao1.Id,
                Nome = "Concorrente 1",
                Reajuste = new Reajuste
                {
                    Salario = 0.1F,
                    Piso = 0.1F,
                    AuxCreche = 0.1F,
                    DescontoVt = 0.06F,
                    VaVr = 0.1F,
                    VaVrFerias = true
                }
            });

            context.Concorrentes.Add(new Concorrente
            {
                NegociacaoId = negociacao2.Id,
                Nome = "Concorrente 2",
                Reajuste = new Reajuste
                {
                    Salario = 0.2F,
                    Piso = 0.2F,
                    AuxCreche = 0.2F,
                    DescontoVt = 0.06F,
                    VaVr = 0.2F,
                    VaVrFerias = true
                }
            });


            context.ContatosSindicatosLaborais.Add(new ContatoSindicatoLaboral
            {
                ContatoId = contato1.Id,
                SindicatoLaboralId = laboral1.Id
            });
            context.ContatosSindicatosLaborais.Add(new ContatoSindicatoLaboral
            {
                ContatoId = contato2.Id,
                SindicatoLaboralId = laboral2.Id
            });

            context.ContatosSindicatosPatronais.Add(new ContatoSindicatoPatronal
            {
                ContatoId = contato3.Id,
                SindicatoPatronalId = patronal1.Id
            });
            context.ContatosSindicatosPatronais.Add(new ContatoSindicatoPatronal
            {
                ContatoId = contato4.Id,
                SindicatoPatronalId = patronal2.Id
            });

            context.SaveChanges();
        }
    }
}
