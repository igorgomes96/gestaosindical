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

            services.AddDbContext<Context>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Base"));
            });

            // services.AddDbContext<Context>(opt =>
            // {
            //     opt.UseMySql(Configuration.GetConnectionString("BaseMySql"));
            // });

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
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Base")));
            // services.AddDbContext<AuthContext>(opt =>
            // {
            //     opt.UseInMemoryDatabase("GestaoSindical");
            // });
            // services.AddDbContext<AuthContext>(options =>
            //     options.UseMySql(Configuration.GetConnectionString("BaseMySql")));

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

            // SeedDataTest(context);

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
                Cnpj = "12312312312312",
                Gestao = "Gestão Teste",
                Nome = "Patronal Teste",
                Site = "Site Teste",
                Telefone1 = "3428282828"
            };
            if (context.SindicatosPatronais.Count() == 0) context.SindicatosPatronais.Add(patronal);

            Endereco endereco = new Endereco
            {
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
                Cnpj = "12312312312312",
                EnderecoId = endereco.Id,
                Nome = "Empresa Teste",
                MassaSalarial = 989,
                QtdaTrabalhadores = 2387,
                SindicatoLaboralId = laboral.Id,
                SindicatoPatronalId = patronal.Id,
            };
            if (context.Empresas.Count() == 0) context.Empresas.Add(empresa);


            Reajuste orcado = new Reajuste { };
            Reajuste negociado = new Reajuste { };
            if (context.Reajustes.Count() == 0)
            {
                context.Reajustes.Add(orcado);
                context.Reajustes.Add(negociado);
            }

            context.SaveChanges();

            Negociacao negociacao = new Negociacao
            {
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

            context.PerguntasPadrao.RemoveRange(context.PerguntasPadrao);
            context.GruposPerguntasPadrao.RemoveRange(context.GruposPerguntasPadrao);

            context.SaveChanges();


            GrupoPerguntaPadrao grupo1 = new GrupoPerguntaPadrao
            {
                Ordem = 1,
                Texto = "Dados Gerais"
            };
            GrupoPerguntaPadrao grupo2 = new GrupoPerguntaPadrao
            {
                Ordem = 2,
                Texto = "Informações Aplicação de Reajustes do Instrumento Coletivo"
            };
            GrupoPerguntaPadrao grupo3 = new GrupoPerguntaPadrao
            {
                Ordem = 3,
                Texto = "Benefícios"
            };
            GrupoPerguntaPadrao grupo4 = new GrupoPerguntaPadrao
            {
                Ordem = 4,
                Texto = "Reajuste Por Tempo De Empresa"
            };
            GrupoPerguntaPadrao grupo5 = new GrupoPerguntaPadrao
            {
                Ordem = 5,
                Texto = "Taxas Negociais"
            };
            GrupoPerguntaPadrao grupo6 = new GrupoPerguntaPadrao
            {
                Ordem = 6,
                Texto = "Jornada De Trabalho E Banco De Horas"
            };
            GrupoPerguntaPadrao grupo7 = new GrupoPerguntaPadrao
            {
                Ordem = 7,
                Texto = "Adicionais Legais"
            };
            GrupoPerguntaPadrao grupo8 = new GrupoPerguntaPadrao
            {
                Ordem = 8,
                Texto = "Faltas Justificadas E Licenças Remuneradas"
            };
            GrupoPerguntaPadrao grupo9 = new GrupoPerguntaPadrao
            {
                Ordem = 9,
                Texto = "Estabilidades Provisórias"
            };
            GrupoPerguntaPadrao grupo10 = new GrupoPerguntaPadrao
            {
                Ordem = 10,
                Texto = "Férias"
            };
            GrupoPerguntaPadrao grupo11 = new GrupoPerguntaPadrao
            {
                Ordem = 11,
                Texto = "Ponto"
            };
            GrupoPerguntaPadrao grupo12 = new GrupoPerguntaPadrao
            {
                Ordem = 12,
                Texto = "Proporção do Banco"
            };
            GrupoPerguntaPadrao grupo13 = new GrupoPerguntaPadrao
            {
                Ordem = 13,
                Texto = "Percentual de Horas Extras"
            };
            GrupoPerguntaPadrao grupo14 = new GrupoPerguntaPadrao
            {
                Ordem = 14,
                Texto = "Adicional Noturno"
            };
            GrupoPerguntaPadrao grupo15 = new GrupoPerguntaPadrao
            {
                Ordem = 15,
                Texto = "Percentual de sobreaviso"
            };
            GrupoPerguntaPadrao grupo16 = new GrupoPerguntaPadrao
            {
                Ordem = 16,
                Texto = "Percentual intrajornada"
            };
            GrupoPerguntaPadrao grupo17 = new GrupoPerguntaPadrao
            {
                Ordem = 17,
                Texto = "Percentual interjornada"
            };
            GrupoPerguntaPadrao grupo18 = new GrupoPerguntaPadrao
            {
                Ordem = 18,
                Texto = "Falta desconta DSR e feriado"
            };
            GrupoPerguntaPadrao grupo20 = new GrupoPerguntaPadrao
            {
                Ordem = 20,
                Texto = "Tempo para não considerar intervalo"
            };
            GrupoPerguntaPadrao grupo21 = new GrupoPerguntaPadrao
            {
                Ordem = 21,
                Texto = "CÓDIGO DO EVENTO E FOLHA DE PAGAMENTO"
            };

            context.GruposPerguntasPadrao.AddRange(new List<GrupoPerguntaPadrao> {
                    grupo1, grupo2, grupo3, grupo4, grupo5, grupo6, grupo7, grupo8,
                    grupo9, grupo10, grupo11
                });
            context.SaveChanges();

            PerguntaPadrao p1 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Nome da Empresa",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p2 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Nome Sindicato Laboral",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p3 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "CNPJ",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p4 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Código do Sindicato",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p5 = new PerguntaPadrao
            {
                Ordem = 5,
                Texto = "Região Abrangida pelo Sindicato",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p6 = new PerguntaPadrao
            {
                Ordem = 6,
                Texto = "Nome Sindicato Patronal",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p7 = new PerguntaPadrao
            {
                Ordem = 7,
                Texto = "UF",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p8 = new PerguntaPadrao
            {
                Ordem = 8,
                Texto = "Instrum. Col. (ACT/CCT)",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p9 = new PerguntaPadrao
            {
                Ordem = 9,
                Texto = "Nome Responsável TH",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p10 = new PerguntaPadrao
            {
                Ordem = 10,
                Texto = "Data base",
                GrupoPerguntaId = grupo1.Id
            };
            PerguntaPadrao p11 = new PerguntaPadrao
            {
                Ordem = 11,
                Texto = "Data da Assembléia ou oficialização da aprovação?",
                GrupoPerguntaId = grupo1.Id
            };

            PerguntaPadrao p12 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Categorias exluídas da convenção/acordo?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p13 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Pagamento retroativo à data Base?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p14 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Aplicar Rejuste para admitidos até qual data?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p15 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Desligados durante período de experiência terão direito a aplicação?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p16 = new PerguntaPadrao
            {
                Ordem = 5,
                Texto = "Competência de aplicação do reajuste único",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p17 = new PerguntaPadrao
            {
                Ordem = 6,
                Texto = "Competência reajuste fracionado 1ª Parcela",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p20 = new PerguntaPadrao
            {
                Ordem = 7,
                Texto = "Competência reajuste fracionado 2ª Parcela",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p21 = new PerguntaPadrao
            {
                Ordem = 8,
                Texto = "Competência reajuste fracionado 3ª Parcela",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p19 = new PerguntaPadrao
            {
                Ordem = 9,
                Texto = "Parcelar para demitidos?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p22 = new PerguntaPadrao
            {
                Ordem = 10,
                Texto = "O Reajuste será aplicado proporcional à admissão após a data base?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p23 = new PerguntaPadrao
            {
                Ordem = 11,
                Texto = "Reajuste será aplicado para empregados desligados após data base?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p24 = new PerguntaPadrao
            {
                Ordem = 12,
                Texto = "Regra de aplicação de Abono CCT",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p25 = new PerguntaPadrao
            {
                Ordem = 13,
                Texto = "Os empregados que tiverem promoção, mérito ou enquadramento após a data base devem ter o reajuste individual compensado no percentual do reajuste coletivo?",
                GrupoPerguntaId = grupo2.Id
            };
            PerguntaPadrao p26 = new PerguntaPadrao
            {
                Ordem = 14,
                Texto = "Pisos Salariais",
                GrupoPerguntaId = grupo2.Id
            };

            PerguntaPadrao p27 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Vale Alimentação/Refeição",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p28 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Vale Alimentação/Refeição nas férias ",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p29 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Vale Alimentação/Refeição horas extras",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p30 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Cesta Básica",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p31 = new PerguntaPadrao
            {
                Ordem = 5,
                Texto = "Auxílio Creche",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p32 = new PerguntaPadrao
            {
                Ordem = 6,
                Texto = "Auxílio a Filho Excepcional",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p33 = new PerguntaPadrao
            {
                Ordem = 7,
                Texto = "Auxlílio Babá",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p34 = new PerguntaPadrao
            {
                Ordem = 8,
                Texto = "Complemento Auxílio Doença",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p35 = new PerguntaPadrao
            {
                Ordem = 9,
                Texto = "Auxílio Funeral",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p36 = new PerguntaPadrao
            {
                Ordem = 10,
                Texto = "Plano odontológico",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p37 = new PerguntaPadrao
            {
                Ordem = 11,
                Texto = "Plano médico",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p38 = new PerguntaPadrao
            {
                Ordem = 12,
                Texto = "Percentual desconto do Vale Transporte",
                GrupoPerguntaId = grupo3.Id
            };
            PerguntaPadrao p39 = new PerguntaPadrao
            {
                Ordem = 13,
                Texto = "Seguro de vida",
                GrupoPerguntaId = grupo3.Id
            };

            PerguntaPadrao p40 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Anuênio",
                GrupoPerguntaId = grupo4.Id
            };
            PerguntaPadrao p41 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Triênio",
                GrupoPerguntaId = grupo4.Id
            };
            PerguntaPadrao p42 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Biênio",
                GrupoPerguntaId = grupo4.Id
            };
            PerguntaPadrao p43 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Quinquênio",
                GrupoPerguntaId = grupo4.Id
            };

            PerguntaPadrao p44 = new PerguntaPadrao
            {
                Ordem = 5,
                Texto = "Sindicato Laboral - Desconto de taxa de fortalecimento/Negociação/Confederativa, dos empregados.",
                GrupoPerguntaId = grupo5.Id
            };
            PerguntaPadrao p45 = new PerguntaPadrao
            {
                Ordem = 6,
                Texto = "Sindicato Laboral - Desconto de taxa de associação/Mensal dos empregados",
                GrupoPerguntaId = grupo5.Id
            };
            PerguntaPadrao p46 = new PerguntaPadrao
            {
                Ordem = 7,
                Texto = "Sindicato Patronal - Taxas de fortalecimento",
                GrupoPerguntaId = grupo5.Id
            };

            PerguntaPadrao p47 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Horas Extras",
                GrupoPerguntaId = grupo6.Id
            };
            PerguntaPadrao p48 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Sobreaviso",
                GrupoPerguntaId = grupo6.Id
            };
            PerguntaPadrao p49 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Banco de Horas",
                GrupoPerguntaId = grupo6.Id
            };
            PerguntaPadrao p50 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Horas Extras 50%",
                GrupoPerguntaId = grupo6.Id
            };
            PerguntaPadrao p51 = new PerguntaPadrao
            {
                Ordem = 5,
                Texto = "Horas Extras 60%",
                GrupoPerguntaId = grupo6.Id
            };
            PerguntaPadrao p52 = new PerguntaPadrao
            {
                Ordem = 6,
                Texto = "Horas Extras 100%",
                GrupoPerguntaId = grupo6.Id
            };
            PerguntaPadrao p53 = new PerguntaPadrao
            {
                Ordem = 7,
                Texto = "Horas Extras 150%",
                GrupoPerguntaId = grupo6.Id
            };

            PerguntaPadrao p54 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Adicional Noturno",
                GrupoPerguntaId = grupo7.Id
            };
            PerguntaPadrao p55 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Adicional de Periculosidade",
                GrupoPerguntaId = grupo7.Id
            };
            PerguntaPadrao p56 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Adicional de Insalubridade",
                GrupoPerguntaId = grupo7.Id
            };

            PerguntaPadrao p57 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Licença Maternidade 180 dias",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p58 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Folgas negociadas",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p59 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Licença Paternidade acima do prazo legal do Art. 473 da CLT",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p60 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Licença Adotante",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p61 = new PerguntaPadrao
            {
                Ordem = 5,
                Texto = "Licença Militar",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p62 = new PerguntaPadrao
            {
                Ordem = 6,
                Texto = "Liberação para amamentação",
                GrupoPerguntaId = grupo8.Id
            };

            PerguntaPadrao p63 = new PerguntaPadrao
            {
                Ordem = 7,
                Texto = "Pré-Aposentadoria",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p64 = new PerguntaPadrao
            {
                Ordem = 8,
                Texto = "Retorno de Férias",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p65 = new PerguntaPadrao
            {
                Ordem = 9,
                Texto = "Cipa",
                GrupoPerguntaId = grupo8.Id
            };
            PerguntaPadrao p66 = new PerguntaPadrao
            {
                Ordem = 10,
                Texto = "Mandado sindical",
                GrupoPerguntaId = grupo8.Id,
            };
            PerguntaPadrao p67 = new PerguntaPadrao
            {
                Ordem = 11,
                Texto = "Auxílio doença",
                GrupoPerguntaId = grupo8.Id,
            };
            PerguntaPadrao p68 = new PerguntaPadrao
            {
                Ordem = 12,
                Texto = "Periodo de Trindidio",
                GrupoPerguntaId = grupo8.Id,
            };
            PerguntaPadrao p69 = new PerguntaPadrao
            {
                Ordem = 13,
                Texto = "Licença maternidade",
                GrupoPerguntaId = grupo8.Id
            };

            PerguntaPadrao p70 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Adcional de Férias acima dos 33,33%",
                GrupoPerguntaId = grupo9.Id
            };
            PerguntaPadrao p71 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Regra de fracionamento de férias",
                GrupoPerguntaId = grupo9.Id
            };
            PerguntaPadrao p72 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Regra para inicio de férias",
                GrupoPerguntaId = grupo9.Id
            };

            PerguntaPadrao p73 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Utiliza ponto por Excessão",
                GrupoPerguntaId = grupo10.Id
            };
            PerguntaPadrao p74 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Banco de Horas",
                GrupoPerguntaId = grupo10.Id
            };
            PerguntaPadrao p75 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Prazo do Banco de Horas",
                GrupoPerguntaId = grupo10.Id
            };
            PerguntaPadrao p76 = new PerguntaPadrao
            {
                Ordem = 4,
                Texto = "Existe proporção do Banco de Horas",
                GrupoPerguntaId = grupo10.Id
            };
            PerguntaPadrao p77 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Regra do Banco de Horas",
                GrupoPerguntaId = grupo10.Id
            };

            PerguntaPadrao p78 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Dia Normal e sábado",
                GrupoPerguntaId = grupo12.Id
            };
            PerguntaPadrao p79 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Domingo e Feriado",
                GrupoPerguntaId = grupo12.Id
            };
            PerguntaPadrao p80 = new PerguntaPadrao
            {
                Ordem = 3,
                Texto = "Pag. Banco sem proporção",
                GrupoPerguntaId = grupo12.Id
            };

            PerguntaPadrao p81 = new PerguntaPadrao
            {
                Ordem = 1,
                Texto = "Dia Normal e sábado",
                GrupoPerguntaId = grupo13.Id
            };
            PerguntaPadrao p82 = new PerguntaPadrao
            {
                Ordem = 2,
                Texto = "Domingo e Feriado",
                GrupoPerguntaId = grupo13.Id
            };
            
            context.PerguntasPadrao.AddRange(new List<PerguntaPadrao> {
                    p1, p2, p3, p4, p5, p6, p7, p8, p9, 
                    p10, p11, p12, p13, p14, p15, p16, p17, p19,
                    p20, p21, p22, p23, p24, p25, p26, p27, p28, p29,
                    p30, p31, p32, p33, p34, p35, p36, p37, p38, p39,
                    p40, p41, p42, p43, p44, p45, p46, p47, p48, p49,
                    p50, p51, p52, p53, p54, p55, p56, p57, p58, p59,
                    p60, p61, p62, p63, p64, p65, p66, p67, p68, p69
                });
            context.SaveChanges();

        }

    }
}
