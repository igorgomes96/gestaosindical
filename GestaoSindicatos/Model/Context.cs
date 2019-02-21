using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Context : DbContext
    {

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemLitigio>()
                .HasOne(a => a.PlanoAcao)
                .WithOne(b => b.ItemLitigio)
                .HasForeignKey<PlanoAcao>(b => b.ItemLitigioId);

            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.Nome);

            modelBuilder.Entity<SindicatoPatronal>()
                .HasIndex(e => e.Nome);

            modelBuilder.Entity<SindicatoLaboral>()
                .HasIndex(e => e.Nome);

            modelBuilder.Entity<ContatoSindicatoLaboral>()
                .HasIndex(e => new { e.ContatoId, e.SindicatoLaboralId })
                .IsUnique();

            modelBuilder.Entity<ContatoSindicatoPatronal>()
                .HasIndex(e => new { e.ContatoId, e.SindicatoPatronalId })
                .IsUnique();

            modelBuilder.Entity<ContatoEmpresa>()
                .HasIndex(e => new { e.ContatoId, e.EmpresaId })
                .IsUnique();

            modelBuilder.Entity<Negociacao>()
                .HasIndex(e => new { e.Ano, e.EmpresaId })
                .IsUnique();

            modelBuilder.Entity<RodadaNegociacao>()
               .HasIndex(e => new { e.Data, e.NegociacaoId });

            modelBuilder.Entity<Litigio>()
               .HasIndex(e => e.Data);

            modelBuilder.Entity<PlanoAcao>()
               .HasIndex(e => e.Data);

            modelBuilder.Entity<EmpresaUsuario>()
                .HasIndex(e => new { e.UserName, e.EmpresaId })
                .IsUnique();

            modelBuilder.Entity<ParcelaReajuste>()
                .HasIndex(e => new { e.Mes, e.ReajusteId, e.TipoReajuste })
                .IsUnique();

            modelBuilder.Entity<Arquivo>()
                .HasIndex(e => new { e.DependencyId, e.DependencyType });

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<SindicatoPatronal> SindicatosPatronais { get; set; }
        public virtual DbSet<SindicatoLaboral> SindicatosLaborais { get; set; }
        public virtual DbSet<Contato> Contatos { get; set; }
        public virtual DbSet<Endereco> Enderecos { get; set; }
        public virtual DbSet<ContatoSindicatoLaboral> ContatosSindicatosLaborais { get; set; }
        public virtual DbSet<ContatoSindicatoPatronal> ContatosSindicatosPatronais { get; set; }
        public virtual DbSet<ContatoEmpresa> ContatosEmpresas { get; set; }
        public virtual DbSet<Negociacao> Negociacoes { get; set; }
        public virtual DbSet<RodadaNegociacao> RodadasNegociacoes { get; set; }
        public virtual DbSet<Litigio> Litigios { get; set; }
        public virtual DbSet<PlanoAcao> PlanosAcao { get; set; }
        public virtual DbSet<Reajuste> Reajustes { get; set; }
        public virtual DbSet<Concorrente> Concorrentes { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<EmpresaUsuario> EmpresasUsuarios { get; set; }
        public virtual DbSet<ParcelaReajuste> ParcelasReajustes { get; set; }
        public virtual DbSet<Arquivo> Arquivos { get; set; }
        public virtual DbSet<ItemLitigio> ItensLitigios { get; set; }
        public virtual DbSet<Relatorio> Relatorios { get; set; }
        public virtual DbSet<GrupoPergunta> GruposPerguntas { get; set; }
        public virtual DbSet<GrupoPerguntaPadrao> GruposPerguntasPadrao { get; set; }
        public virtual DbSet<PerguntaPadrao> PerguntasPadrao { get; set; }
        public virtual DbSet<RespostaRelatorio> RespostasRelatorio { get; set; }
    }
}
