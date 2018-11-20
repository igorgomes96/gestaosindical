using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using GestaoSindicatos.Model.Dashboard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class DashboardService
    {
        private readonly Context _db;
        private readonly EmpresasService _empresasService;

        public DashboardService(Context db, EmpresasService empresasService)
        {
            _db = db;
            _empresasService = empresasService;
        }

        public ICollection<ChartData> StatusPorMassaSalarial<TKey>(ClaimsPrincipal claims, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return StatusPorMassaSalarial(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano, groupByKey);
        }

        public ICollection<ChartData> StatusPorQtdaTrabalhadores<TKey>(ClaimsPrincipal claims, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return StatusPorQtdaTrabalhadores(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano, groupByKey);
        }


        public ICollection<ChartData> StatusPorMassaSalarial<TKey>(List<int> empresasIds, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return _db.Negociacoes
                .Where(x => x.Ano == ano && x.EmpresaId.HasValue && empresasIds.Contains(x.EmpresaId.Value))
                .Include(x => x.Empresa)
                .GroupBy(groupByKey)
                .Select(x => new ChartData
                {
                    Y = x.Sum(v => v.Empresa.MassaSalarial),
                    Label = x.Key.ToString()
                }).ToList();
        }

        public ICollection<ChartData> StatusPorQtdaTrabalhadores<TKey>(List<int> empresasIds, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return _db.Negociacoes
                .Where(x => x.Ano == ano && x.EmpresaId.HasValue && empresasIds.Contains(x.EmpresaId.Value))
                .Include(x => x.Empresa)
                .GroupBy(groupByKey)
                .Select(x => new ChartData
                {
                    Y = x.Sum(v => v.Empresa.QtdaTrabalhadores),
                    Label = x.Key.ToString()
                }).ToList();
        }

        public IQueryable<PlanoAcao> PlanosByClaims(IQueryable<PlanoAcao> planos, ClaimsPrincipal claims)
        {
            if (claims.IsInRole(Roles.ADMIN))
                return planos;

            IQueryable<Empresa> empresas = _empresasService.Query(claims);
            return planos.Where(x => empresas.Any(y => y.SindicatoLaboralId == x.LaboralId || y.SindicatoPatronalId == x.PatronalId));

        }

        public ICollection<ChartData> StatusPorPlanosAcao(ClaimsPrincipal claims, int ano)
        {
            IQueryable<PlanoAcao> planos = _db.PlanosAcao
                    .Where(x => x.Data.Year == ano);

            return PlanosByClaims(planos, claims)
                .GroupBy(x => x.Status)
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = x.Key.ToString()
                }).ToList();
        }

        public ICollection<ChartData> PlanosReferenteA(ClaimsPrincipal claims, int ano)
        {
            IQueryable<PlanoAcao> planos = _db.PlanosAcao
                    .Where(x => x.Data.Year == ano);

            return PlanosByClaims(planos, claims)
                .GroupBy(x => x.Referente)
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = x.Key.ToString()
                }).ToList();

        }

        public ICollection<ChartData> PlanosProcedencia(ClaimsPrincipal claims, int ano)
        {
            IQueryable<PlanoAcao> planos = _db.PlanosAcao
                    .Where(x => x.Data.Year == ano && x.Status == StatusPlanoAcao.Solucionado);

            return PlanosByClaims(planos, claims)
                .GroupBy(x => x.Procedencia ? "Procedente" : "Improcedente")
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = x.Key.ToString()
                }).ToList();

        }

        public IQueryable<Litigio> LitigiosByClaims(IQueryable<Litigio> litigios, ClaimsPrincipal claims)
        {
            if (claims.IsInRole(Roles.ADMIN))
                return litigios;

            IQueryable<Empresa> empresas = _empresasService.Query(claims);
            return litigios.Where(x => empresas.Any(y => y.Id == x.EmpresaId));

        }

        public ICollection<ChartData> LitigiosGroup<TKey>(ClaimsPrincipal claims, int ano, Expression<Func<Litigio, TKey>> groupByKey)
        {
            IQueryable<Litigio> litigios = _db.Litigios
                    .Where(x => x.Data.Year == ano);

            return LitigiosByClaims(litigios, claims)
                .GroupBy(groupByKey)
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = x.Key.ToString()
                }).ToList();

        }

        public ICollection<ChartData> LitigiosMensal(ClaimsPrincipal claims, int ano)
        {
            IQueryable<Litigio> litigios = _db.Litigios
                    .Where(x => x.Data.Year == ano);

            return LitigiosByClaims(litigios, claims)
                .GroupBy(x => x.Data.Month - 1)
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = Enum.Parse(typeof(Mes), x.Key.ToString()).ToString()
                }).ToList();

        }

        public ICollection<ChartData> LitigiosPorEmpresa(ClaimsPrincipal claims, int ano)
        {
            IQueryable<Litigio> litigios = _db.Litigios
                    .Where(x => x.Data.Year == ano);

            return LitigiosByClaims(litigios, claims)
                .Include(x => x.Empresa)
                .GroupBy(x => x.Empresa.Nome)
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = x.Key.ToString()
                }).ToList();

        }

        public ICollection<ChartData> QtdaReunioes<TKey>(ClaimsPrincipal claims, int ano, Expression<Func<RodadaNegociacao, TKey>> groupByKey)
        {
            return QtdaReunioes(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano, groupByKey);
        }

        public ICollection<ChartData> QtdaReunioes<TKey>(List<int> empresasIds, int ano, Expression<Func<RodadaNegociacao, TKey>> groupByKey)
        {
            return _db.RodadasNegociacoes
                .Include(x => x.Negociacao)
                    .ThenInclude(x => x.SindicatoLaboral)
                .Include(x => x.Negociacao)
                    .ThenInclude(x => x.SindicatoPatronal)
                .Include(x => x.Negociacao)
                    .ThenInclude(x => x.Empresa)
                        .ThenInclude(x => x.Endereco)
                .Where(x => x.Data.Year == ano && x.Negociacao.EmpresaId.HasValue && empresasIds.Contains(x.Negociacao.EmpresaId.Value))
                .GroupBy(groupByKey)
                .Select(x => new ChartData
                {
                    Y = x.Count(),
                    Label = x.Key.ToString()
                }).ToList();
        }


        public ICollection<ChartData> CustosEmViagens(ClaimsPrincipal claims, int ano)
        {
            return CustosEmViagens(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano);
        }
        public ICollection<ChartData> CustosEmViagens(List<int> empresasIds, int ano)
        {
            return _db.RodadasNegociacoes
                .Where(x => x.Data.Year == ano)
                .Include(x => x.Negociacao)
                .Where(x => x.Negociacao.EmpresaId.HasValue && empresasIds.Contains(x.Negociacao.EmpresaId.Value) && x.CustosViagens.HasValue)
                .GroupBy(x => x.Data.Month - 1)
                .Select(x => new ChartData
                {
                    Y = x.Sum(v => v.CustosViagens.Value),
                    Label = Enum.Parse(typeof(Mes), x.Key.ToString()).ToString()
                }).ToList();

        }

        public ICollection<ChartDataset> MediaReajustes(ClaimsPrincipal claims, int ano)
        {
            return MediaReajustes(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano);
        }


        public ICollection<ChartDataset> MediaReajustes(List<int> empresasIds, int ano)
        {
            var negociacoes = _db.Negociacoes
               .Where(x => x.Ano == ano && x.EmpresaId.HasValue && empresasIds.Contains(x.EmpresaId.Value))
               .Include(x => x.Orcado)
               .Include(x => x.Negociado)
               .Include(x => x.Concorrentes)
               .ThenInclude(c => c.Reajuste);

            if (negociacoes.Count() > 0)
            {
                ChartDataset orcado = new ChartDataset
                {
                    Label = "Orçado",
                    Data = new List<ChartData>
                    {
                        new ChartData { Y = Math.Round(negociacoes.Average(x => x.Orcado.Piso), 2), Label = "Piso" },
                        new ChartData { Y = Math.Round(negociacoes.Average(x => x.Orcado.Salario), 2), Label = "Salário" },
                        new ChartData { Y = Math.Round(negociacoes.Average(x => x.Orcado.VaVr), 2), Label = "VA/VR" },
                        new ChartData { Y = Math.Round(negociacoes.Average(x => x.Orcado.AuxCreche), 2), Label = "Aux. Creche" }
                    }
                };

                ChartDataset negociado = new ChartDataset
                {
                    Label = "Negociado",
                    Data = new List<ChartData>
                        {
                            new ChartData { Y = Math.Round(negociacoes.Average(x => x.Negociado.Piso), 2), Label = "Piso" },
                            new ChartData { Y = Math.Round(negociacoes.Average(x => x.Negociado.Salario), 2), Label = "Salário" },
                            new ChartData { Y = Math.Round(negociacoes.Average(x => x.Negociado.VaVr), 2), Label = "VA/VR" },
                            new ChartData { Y = Math.Round(negociacoes.Average(x => x.Negociado.AuxCreche), 2), Label = "Aux. Creche" }
                        }
                };

                List<Negociacao> negociaoesComConcorrentes = negociacoes.Where(x => x.Concorrentes.Count() > 0).ToList();
                ChartDataset concorrentes = new ChartDataset
                {
                    Label = "Concorrentes",
                    Data = new List<ChartData> {
                            new ChartData { Y = 0, Label = "Piso" },
                            new ChartData { Y = 0, Label = "Salário" },
                            new ChartData { Y = 0, Label = "VA/VR" },
                            new ChartData { Y = 0, Label = "Aux. Creche" }
                        }
                };
                if (negociaoesComConcorrentes.Count() > 0)
                {
                    concorrentes = new ChartDataset
                    {
                        Label = "Concorrentes",
                        Data = new List<ChartData> {
                            new ChartData { Y = Math.Round(negociaoesComConcorrentes.Average(x => x.Concorrentes.Average(y => y.Reajuste.Piso)), 2), Label = "Piso" },
                            new ChartData { Y = Math.Round(negociaoesComConcorrentes.Average(x => x.Concorrentes.Average(y => y.Reajuste.Salario)), 2), Label = "Salário" },
                            new ChartData { Y = Math.Round(negociaoesComConcorrentes.Average(x => x.Concorrentes.Average(y => y.Reajuste.VaVr)), 2), Label = "VA/VR" },
                            new ChartData { Y = Math.Round(negociaoesComConcorrentes.Average(x => x.Concorrentes.Average(y => y.Reajuste.AuxCreche)), 2), Label = "Aux. Creche" }
                        }
                    };
                }

                return new List<ChartDataset> { orcado, negociado, concorrentes };
            }

            return new List<ChartDataset>();

        }
    }
}
