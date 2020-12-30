using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{

    public class RodadasService : CrudService<RodadaNegociacao>
    {
        private readonly Context _db;
        private readonly ArquivosService _arquivosService;
        private readonly List<Color> _colors;

        public RodadasService(Context db, ArquivosService arquivosService) : base(db)
        {
            _db = db;
            _arquivosService = arquivosService;
            _colors = new List<Color>
            {
                { new Color { Primary = "#ad2121", Secondary = "#FAE3E3" } }, //red
                { new Color { Primary = "#1e90ff", Secondary = "#D1E8FF" } }, //blue
                { new Color { Primary = "#e3bc08", Secondary = "#FDF1BA" } }  //yellow
            };
        }

        public override RodadaNegociacao Add(RodadaNegociacao entity)
        {
            int qtdaRodadas = Count(r => r.NegociacaoId == entity.NegociacaoId);
            entity.Numero = qtdaRodadas + 1;
            entity = base.Add(entity);
            _db.Entry(entity).Reference(r => r.Negociacao).Load();
            entity.Negociacao.QtdaRodadas = _db.RodadasNegociacoes.Count(r => r.NegociacaoId == entity.NegociacaoId);
            _db.SaveChanges();
            return entity;
        }

        public override RodadaNegociacao Delete(params object[] key)
        {
            _arquivosService.DeleteFiles(DependencyFileType.RodadaNegociacao, (int)key[0]);
            RodadaNegociacao rodada = Find(key);
            _db.Entry(rodada).Reference(r => r.Negociacao).Load();
            rodada.Negociacao.QtdaRodadas--;

            _db.RodadasNegociacoes.Where(x => x.Numero > rodada.Numero)
                .ToList().ForEach(r =>
                {
                    r.Numero--;
                    Update(r, r.Id);
                });
            _db.SaveChanges();
            return base.Delete(key);
        }


        public IQueryable<Negociacao> GetNegociacoes(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return _db.Negociacoes
                    .Where(n => _db.EmpresasUsuarios.Any(u => u.UserName == claims.Identity.Name && u.EmpresaId == n.EmpresaId));

            return _db.Negociacoes;
        }

        public ICollection<CalendarEvent> GetCalendar(ClaimsPrincipal claims, int mes)
        {
            Dictionary<int, Negociacao> negociacoes = GetNegociacoes(claims)
                .Include(n => n.Empresa).ThenInclude(e => e.Endereco)
                .ToList().ToDictionary(x => x.Id);

            return Query(r => r.Data.Month == mes).OrderBy(x => x.Data).ToList()
                .Where(r => negociacoes.ContainsKey(r.NegociacaoId))
                .Select((r, i) =>
                {
                    Negociacao neg = negociacoes[r.NegociacaoId];
                    return new CalendarEvent
                    {
                        AllDay = true,
                        Start = r.Data,
                        End = r.Data,
                        Color = _colors.ElementAt(i % _colors.Count),
                        Title = $"#{r.NegociacaoId} {neg.Empresa.Nome}, {neg.Empresa.Endereco.Cidade} - {neg.Empresa.Endereco.UF}"
                    };
                }).ToList();
        }

        public override void Delete(Expression<Func<RodadaNegociacao, bool>> query)
        {
            Query(query).ToList().ForEach(r => Delete(r.Id));
        }
    }
}
