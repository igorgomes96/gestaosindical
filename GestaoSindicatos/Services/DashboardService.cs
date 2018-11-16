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

        public ICollection<PieChartData> StatusPorMassaSalarial<TKey>(ClaimsPrincipal claims, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return StatusPorMassaSalarial(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano, groupByKey);
        }

        public ICollection<PieChartData> StatusPorQtdaTrabalhadores<TKey>(ClaimsPrincipal claims, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return StatusPorQtdaTrabalhadores(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), ano, groupByKey);
        }


        public ICollection<PieChartData> StatusPorMassaSalarial<TKey>(List<int> empresasIds, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return _db.Negociacoes
                .Where(x => x.Ano == ano && x.EmpresaId.HasValue && empresasIds.Contains(x.EmpresaId.Value))
                .Include(x => x.Empresa)
                .GroupBy(groupByKey)
                .Select(x => new PieChartData
                {
                    Y = x.Sum(v => v.Empresa.MassaSalarial),
                    Label = x.Key.ToString()
                }).ToList();
        }

        public ICollection<PieChartData> StatusPorQtdaTrabalhadores<TKey>(List<int> empresasIds, int ano, Expression<Func<Negociacao, TKey>> groupByKey)
        {
            return _db.Negociacoes
                .Where(x => x.Ano == ano && x.EmpresaId.HasValue && empresasIds.Contains(x.EmpresaId.Value))
                .Include(x => x.Empresa)
                .GroupBy(groupByKey)
                .Select(x => new PieChartData
                {
                    Y = x.Sum(v => v.Empresa.QtdaTrabalhadores),
                    Label = x.Key.ToString()
                }).ToList();
        }
    }
}
