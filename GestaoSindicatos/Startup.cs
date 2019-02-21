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
using Microsoft.AspNetCore.Identity.UI.Services;
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
            // services.AddDbContext<Context>(opt =>
            // {
            //     opt.UseInMemoryDatabase("GestaoSindical");
            // });

            // services.AddDbContext<Context>(opt =>
            // {
            //     opt.UseSqlServer(Configuration.GetConnectionString("Base"));
            // });

            services.AddDbContext<Context>(opt =>
            {
                opt.UseMySql(Configuration.GetConnectionString("BaseMySql"));
            });

            services.AddScoped(typeof(CrudService<>));
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
            services.AddScoped<ReajustesService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<ItensLitigiosService>();
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


            // Authentication
            // services.AddDbContext<AuthContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("Base")));
            // services.AddDbContext<AuthContext>(opt =>
            // {
            //     opt.UseInMemoryDatabase("GestaoSindical");
            // });
            services.AddDbContext<AuthContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("BaseMySql")));

            services.AddTransient<IEmailSender, EmailSender>(i =>
                new EmailSender(
                    Configuration["EmailSender:Host"],
                    Configuration.GetValue<int>("EmailSender:Port"),
                    Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    Configuration.GetValue<bool>("EmailSender:UseDefaultCredentials"),
                    Configuration["EmailSender:UserName"],
                    Configuration["EmailSender:Password"]
                )
            );

            // Ativando a utilização do ASP.NET Identity, a fim de
            // permitir a recuperação de seus objetos via injeção de
            // dependências
            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            }).AddEntityFrameworkStores<AuthContext>()
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Context context,
            AuthContext authContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedDataTest(context);

            // Criação de estruturas, usuários e permissões
            // na base do ASP.NET Identity Core (caso ainda não
            // existam)
            new IdentityInitializer(authContext, userManager, roleManager).Initialize();

            loggerFactory.AddDebug();
            loggerFactory.AddEventLog();

            app.UseCors("AllowAll");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        private void SeedDataTest(Context context)
        {
            SindicatoLaboral laboral = new SindicatoLaboral
            {
                Id = 1,
                Cnpj = "12312312312312",
                Database = Mes.Abril,
                Federacao = "Federação Teste",
                Gestao = "Gestão Teste",
                Nome = "Laboral Teste",
                Site = "Site Teste",
                Telefone1 = "3428282828"
            };
            if (context.SindicatosLaborais.Count() == 0) context.SindicatosLaborais.Add(laboral);

            SindicatoPatronal patronal = new SindicatoPatronal
            {
                Id = 1,
                Cnpj = "12312312312312",
                Gestao = "Gestão Teste",
                Nome = "Patronal Teste",
                Site = "Site Teste",
                Telefone1 = "3428282828"
            };
            if (context.SindicatosPatronais.Count() == 0) context.SindicatosPatronais.Add(patronal);

            Endereco endereco = new Endereco
            {
                Id = 1,
                Bairro = "Bairro Teste",
                Cidade = "Araguari",
                Logradouro = "Rua Teste",
                Numero = "7878",
                UF = "MG"
            };
            if (context.Enderecos.Count() == 0) context.Enderecos.Add(endereco);

            context.SaveChanges();

            Empresa empresa = new Empresa
            {
                Id = 1,
                Cnpj = "12312312312312",
                EnderecoId = endereco.Id,
                Nome = "Empresa Teste",
                MassaSalarial = 989,
                QtdaTrabalhadores = 2387,
                SindicatoLaboralId = laboral.Id,
                SindicatoPatronalId = patronal.Id,
            };
            if (context.Empresas.Count() == 0) context.Empresas.Add(empresa);


            Reajuste orcado = new Reajuste { Id = 1 };
            Reajuste negociado = new Reajuste { Id = 2 };
            if (context.Reajustes.Count() == 0)
            {
                context.Reajustes.Add(orcado);
                context.Reajustes.Add(negociado);
            }

            context.SaveChanges();

            Negociacao negociacao = new Negociacao
            {
                Id = 1,
                Ano = 2019,
                EmpresaId = 1,
                MassaSalarial = 989,
                QtdaTrabalhadores = 2387,
                SindicatoLaboralId = 1,
                SindicatoPatronalId = 1,
                OrcadoId = orcado.Id,
                NegociadoId = negociado.Id
            };
            if (context.Negociacoes.Count() == 0) context.Negociacoes.Add(negociacao);

            context.SaveChanges();

            if (context.RodadasNegociacoes.Count() == 0)
                context.RodadasNegociacoes.Add(new RodadaNegociacao
                {
                    CustosViagens = 1000,
                    Data = DateTime.Today,
                    NegociacaoId = 1,
                    Numero = 1,
                    Resumo = "Rodada de teste"
                });

            context.SaveChanges();

            if (context.GruposPerguntasPadrao.Count() == 0) {
                GrupoPerguntaPadrao grupo1 = new GrupoPerguntaPadrao {
                    Ordem = 1,
                    Texto = "Dados Gerais"
                };
                GrupoPerguntaPadrao grupo2 = new GrupoPerguntaPadrao {
                    Ordem = 2,
                    Texto = "Informações Aplicação de Reajustes do Instrumento Coletivo"
                };
                GrupoPerguntaPadrao grupo3 = new GrupoPerguntaPadrao {
                    Ordem = 3,
                    Texto = "Benefícios"
                };
                GrupoPerguntaPadrao grupo4 = new GrupoPerguntaPadrao {
                    Ordem = 4,
                    Texto = "Reajuste Por Tempo De Empresa"
                };
                GrupoPerguntaPadrao grupo5 = new GrupoPerguntaPadrao {
                    Ordem = 5,
                    Texto = "Taxas Negociais"
                };
                GrupoPerguntaPadrao grupo6 = new GrupoPerguntaPadrao {
                    Ordem = 6,
                    Texto = "Jornada De Trabalho E Banco De Horas"
                };
                GrupoPerguntaPadrao grupo7 = new GrupoPerguntaPadrao {
                    Ordem = 7,
                    Texto = "Adicionais Legais"
                };
                GrupoPerguntaPadrao grupo8 = new GrupoPerguntaPadrao {
                    Ordem = 8,
                    Texto = "Faltas Justificadas E Licenças Remuneradas"
                };
                GrupoPerguntaPadrao grupo9 = new GrupoPerguntaPadrao {
                    Ordem = 9,
                    Texto = "Estabilidades Provisórias"
                };
                GrupoPerguntaPadrao grupo10 = new GrupoPerguntaPadrao {
                    Ordem = 10,
                    Texto = "Férias"
                };
                GrupoPerguntaPadrao grupo11 = new GrupoPerguntaPadrao {
                    Ordem = 11,
                    Texto = "Ponto"
                };

                context.GruposPerguntasPadrao.AddRange(new List<GrupoPerguntaPadrao> {
                    grupo1, grupo2, grupo3, grupo4, grupo5, grupo6, grupo7, grupo8,
                    grupo9, grupo10, grupo11  
                });
                context.SaveChanges();

                PerguntaPadrao p1 = new PerguntaPadrao {
                    Ordem = 1,
                    Texto = "Nome da Empresa",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p2 = new PerguntaPadrao {
                    Ordem = 2,
                    Texto = "Nome Sindicato Laboral",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p3 = new PerguntaPadrao {
                    Ordem = 3,
                    Texto = "CNPJ",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p4 = new PerguntaPadrao {
                    Ordem = 4,
                    Texto = "Código do Sindicato",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p5 = new PerguntaPadrao {
                    Ordem = 5,
                    Texto = "Região Abrangida pelo Sindicato",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p6 = new PerguntaPadrao {
                    Ordem = 6,
                    Texto = "Nome Sindicato Patronal",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p7 = new PerguntaPadrao {
                    Ordem = 7,
                    Texto = "UF",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p8 = new PerguntaPadrao {
                    Ordem = 8,
                    Texto = "Instrum. Col. (ACT/CCT)",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p9 = new PerguntaPadrao {
                    Ordem = 9,
                    Texto = "Nome Responsável TH",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p10 = new PerguntaPadrao {
                    Ordem = 10,
                    Texto = "Data base",
                    GrupoPerguntaId = grupo1.Id
                };
                PerguntaPadrao p11 = new PerguntaPadrao {
                    Ordem = 11,
                    Texto = "Data da Assembléia ou oficialização da aprovação?",
                    GrupoPerguntaId = grupo1.Id
                };

                PerguntaPadrao p12 = new PerguntaPadrao {
                    Ordem = 12,
                    Texto = "Categorias exluídas da convenção/acordo?",
                    GrupoPerguntaId = grupo2.Id
                };
                PerguntaPadrao p13 = new PerguntaPadrao {
                    Ordem = 13,
                    Texto = "Pagamento retroativo à data Base?",
                    GrupoPerguntaId = grupo2.Id
                };
                PerguntaPadrao p14 = new PerguntaPadrao {
                    Ordem = 14,
                    Texto = "Aplicar Rejuste para admitidos até qual data?",
                    GrupoPerguntaId = grupo2.Id
                };
                PerguntaPadrao p15 = new PerguntaPadrao {
                    Ordem = 15,
                    Texto = "Desligados durante período de experiência terão direito a aplicação?",
                    GrupoPerguntaId = grupo2.Id
                };
                PerguntaPadrao p16 = new PerguntaPadrao {
                    Ordem = 16,
                    Texto = "Competência de aplicação do reajuste único",
                    GrupoPerguntaId = grupo2.Id
                };

                context.PerguntasPadrao.AddRange(new List<PerguntaPadrao> {
                    p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16
                });
                context.SaveChanges();
            }

        }

    }
}
