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
using MySql.Data.EntityFrameworkCore;

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
                opt.UseMySQL(Configuration.GetConnectionString("BaseMySql"));
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
            services.AddDbContext<AuthContext>(opt =>
            {
                opt.UseInMemoryDatabase("GestaoSindical");
            });

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
            context.SindicatosLaborais.Add(new SindicatoLaboral
            {
                Cnpj = "12312312312312",
                Database = Mes.Abril,
                Federacao = "Federação Teste",
                Gestao = "Gestão Teste",
                Nome = "Laboral Teste",
                Site = "Site Teste",
                Telefone1 = "3428282828"
            });

            context.SindicatosPatronais.Add(new SindicatoPatronal
            {
                Cnpj = "12312312312312",
                Gestao = "Gestão Teste",
                Nome = "Patronal Teste",
                Site = "Site Teste",
                Telefone1 = "3428282828"
            });

            context.Enderecos.Add(new Endereco
            {
                Bairro = "Bairro Teste",
                Cidade = "Araguari",
                Logradouro = "Rua Teste",
                Numero = "7878",
                UF = "MG"
            });

            context.Empresas.Add(new Empresa
            {
                Cnpj = "12312312312312",
                EnderecoId = 1,
                Nome = "Empresa Teste",
                MassaSalarial = 989,
                QtdaTrabalhadores = 2387,
                SindicatoLaboralId = 1,
                SindicatoPatronalId = 1,
            });

            context.Reajustes.Add(new Reajuste { });
            context.Reajustes.Add(new Reajuste { });

            context.Negociacoes.Add(new Negociacao
            {
                Ano = 2019,
                EmpresaId = 1,
                MassaSalarial = 989,
                QtdaTrabalhadores = 2387,
                SindicatoLaboralId = 1,
                SindicatoPatronalId = 1,
                OrcadoId = 1,
                NegociadoId = 2
            });

            context.RodadasNegociacoes.Add(new RodadaNegociacao {
                CustosViagens = 1000,
                Data = DateTime.Today,
                NegociacaoId = 1,
                Numero = 1,
                Resumo = "Rodada de teste"
            });

            context.SaveChanges();
        }

    }
}
