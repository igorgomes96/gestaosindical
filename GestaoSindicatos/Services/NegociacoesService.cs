using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Linq.Expressions;
using GestaoSindicatos.Auth;

namespace GestaoSindicatos.Services
{
    public class NegociacoesService : CrudService<Negociacao>
    {
        private readonly Context _db;
        private readonly RodadasService _rodadasService;
        private readonly CrudService<Reajuste> _reajustesService;

        public NegociacoesService(Context db, RodadasService rodadasService, CrudService<Reajuste> reajustesService) : base(db)
        {
            _db = db;
            _rodadasService = rodadasService;
            _reajustesService = reajustesService;
        }

        private Expression<Func<Negociacao, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (Negociacao n) => _db.EmpresasUsuarios.Any(u => u.UserName == claims.Identity.Name && u.EmpresaId == n.EmpresaId);
        }

        public IQueryable<Negociacao> Query(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query().Where(QueryUser(claims));

            return base.Query();
        }

        public IQueryable<Negociacao> Query(Expression<Func<Negociacao, bool>> query, ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public override Negociacao Find(params object[] key)
        {
            Negociacao negociacao = base.Find(key);
            _db.Entry(negociacao).Reference(n => n.Empresa).Load();
            _db.Entry(negociacao).Reference(n => n.SindicatoLaboral).Load();
            _db.Entry(negociacao).Reference(n => n.SindicatoPatronal).Load();
            _db.Entry(negociacao).Reference(n => n.Orcado).Load();
            _db.Entry(negociacao).Reference(n => n.Negociado).Load();
            return negociacao;
        }

        public override Negociacao Update(Negociacao entity, params object[] key)
        {
            Negociacao neg = base.Update(entity, key);

            if (entity.Negociado != null)
            {
                _reajustesService.Update(entity.Negociado, entity.NegociadoId);
            }

            if (entity.Orcado != null)
            {
                _reajustesService.Update(entity.Orcado, entity.OrcadoId);
            }

            return neg;
        }

        public override Negociacao Delete(params object[] key)
        {
            // Remove os concorrentes e seus reajustes
            _db.Concorrentes.Where(c => c.NegociacaoId == (int)key[0])
                .ToList().ForEach(c =>
                {
                    _db.Reajustes.Remove(_db.Reajustes.Find(c.ReajusteId));
                    _db.Concorrentes.Remove(c);
                });

            // Remove as rodadas
            _rodadasService.Delete(r => r.NegociacaoId == (int)key[0]);

            Negociacao neg = base.Delete(key);
            if (neg != null) {
                if (neg.OrcadoId.HasValue) {
                    _reajustesService.Delete(neg.OrcadoId.Value);
                }
                if (neg.NegociadoId.HasValue)
                {
                    _reajustesService.Delete(neg.NegociadoId.Value);
                }
            }
            return neg;
        }

        public RodadaNegociacao AbrirRodada(int idNegociacao)
        {
            if (!Exist(idNegociacao))
                return null;

            RodadaNegociacao rodada = new RodadaNegociacao
            {
                Data = DateTime.Now,
                NegociacaoId = idNegociacao
            };

            _rodadasService.Add(rodada);
            return rodada;
        }

        public ICollection<RodadaNegociacao> GetRodadasNegociacoes(int negociacaoId)
        {
            Negociacao negociacao = Find(negociacaoId);
            if (negociacao == null)
                return null;

            negociacao.RodadasNegociacoes = new List<RodadaNegociacao>();
            _db.Entry(negociacao).Collection(n => n.RodadasNegociacoes).Load();

            return negociacao.RodadasNegociacoes;
        }

        public RodadaNegociacao NovaRodada(int negociacaoId, RodadaNegociacao rodada)
        {
            Negociacao negociacao = Find(negociacaoId);
            if (negociacao == null) return null;

            _db.Entry(negociacao).Collection(n => n.RodadasNegociacoes);
            rodada.NegociacaoId = negociacaoId;
            rodada.Numero = negociacao.RodadasNegociacoes.Count + 1;

            _rodadasService.Add(rodada);
            negociacao.QtdaRodadas = rodada.Numero;
            _db.SaveChanges();

            return rodada;
        }

        public (int, int) RangeYears()
        {
            if (_db.Negociacoes.Count() == 0)
                return (DateTime.Today.Year, DateTime.Today.Year);
            return (_db.Negociacoes.Min(x => x.Ano), _db.Negociacoes.Max(x => x.Ano));
        }

    }
}
