using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Linq.Expressions;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Model.Dashboard;

namespace GestaoSindicatos.Services
{
    public class NegociacoesService : CrudService<Negociacao>
    {
        private readonly Context _db;
        private readonly RodadasService _rodadasService;
        private readonly CrudService<Reajuste> _reajustesService;
        private readonly EmpresasService _empresasService;

        public NegociacoesService(Context db, RodadasService rodadasService, CrudService<Reajuste> reajustesService,
            EmpresasService empresasService) : base(db)
        {
            _db = db;
            _rodadasService = rodadasService;
            _reajustesService = reajustesService;
            _empresasService = empresasService;
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
            if (negociacao.Empresa != null)
                _db.Entry(negociacao.Empresa).Reference(e => e.Endereco).Load();
            _db.Entry(negociacao).Reference(n => n.SindicatoLaboral).Load();
            _db.Entry(negociacao).Reference(n => n.SindicatoPatronal).Load();
            _db.Entry(negociacao).Reference(n => n.Orcado).Load();
            _db.Entry(negociacao.Orcado).Collection(n => n.Parcelas).Load();
            _db.Entry(negociacao).Reference(n => n.Negociado).Load();
            _db.Entry(negociacao.Negociado).Collection(n => n.Parcelas).Load();
            return negociacao;
        }

        private void UpdateEmpresaValores(Negociacao negociacao)
        {
            Negociacao ultimaNegociacao = _db.Negociacoes.FirstOrDefault(x => x.EmpresaId == negociacao.EmpresaId && x.Ano == _db.Negociacoes.Max(y => y.Ano));
            if (ultimaNegociacao != null && ultimaNegociacao.Id == negociacao.Id)
            {
                if (!_db.Entry(negociacao).Reference(n => n.Empresa).IsLoaded)
                    _db.Entry(negociacao).Reference(n => n.Empresa).Load();

                negociacao.Empresa.MassaSalarial = negociacao.MassaSalarial;
                negociacao.Empresa.QtdaTrabalhadores = negociacao.QtdaTrabalhadores;
                _db.SaveChanges();
            }
        }

        public override Negociacao Add(Negociacao entity)
        {
            Negociacao neg = base.Add(entity);
            UpdateEmpresaValores(neg);
            return neg;
        }

        public override void Add(ICollection<Negociacao> entities)
        {
            entities.ToList().ForEach(e => Add(e));
        }

        public override Negociacao Update(Negociacao entity, params object[] key)
        {
            Negociacao neg = base.Update(entity, key);

            UpdateEmpresaValores(neg);

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

        public ICollection<RodadaNegociacao> ProximasViagens(ClaimsPrincipal claims, int count = 5)
        {
            return ProximasViagens(_empresasService.Query(claims)
                .Select(x => x.Id).ToList(), count);
        }

        public ICollection<RodadaNegociacao> ProximasViagens(List<int> empresasId, int count = 5)
        {
            return _db.RodadasNegociacoes
                .Where(x => x.Data >= DateTime.Today)
                .Include(x => x.Negociacao).ThenInclude(x => x.Empresa).ThenInclude(x => x.Endereco)
                .Where(x => x.Negociacao.EmpresaId.HasValue && empresasId.Contains(x.Negociacao.EmpresaId.Value))
                .Take(count)
                .OrderBy(x => x.Data)
                .ToList();
        }


    }
}
