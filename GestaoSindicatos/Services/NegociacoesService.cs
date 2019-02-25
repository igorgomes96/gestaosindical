using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Linq.Expressions;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Model.Dashboard;
using GestaoSindicatos.Exceptions;

namespace GestaoSindicatos.Services
{
    public class NegociacoesService : CrudService<Negociacao>
    {
        private readonly Context _db;
        private readonly RodadasService _rodadasService;
        private readonly ReajustesService _reajustesService;
        private readonly EmpresasService _empresasService;
        private readonly CrudService<ParcelaReajuste> _parcelasService;
        private readonly ArquivosService _arquivosService;

        public NegociacoesService(Context db, RodadasService rodadasService, ReajustesService reajustesService,
            EmpresasService empresasService, CrudService<ParcelaReajuste> parcelasService,
            ArquivosService arquivosService) : base(db)
        {
            _db = db;
            _rodadasService = rodadasService;
            _reajustesService = reajustesService;
            _empresasService = empresasService;
            _parcelasService = parcelasService;
            _arquivosService = arquivosService;
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
            if (negociacao.Orcado != null)
                _db.Entry(negociacao.Orcado).Collection(n => n.Parcelas).Load();
            _db.Entry(negociacao).Reference(n => n.Negociado).Load();
            if (negociacao.Negociado != null)
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

        public override void Delete(Expression<Func<Negociacao, bool>> query)
        {
            Query(query).ToList().ForEach(n => Delete(n.Id));
        }

        public override Negociacao Delete(params object[] key)
        {

            Negociacao negociacao = Find(key);

            if (negociacao == null) throw new NotFoundException();

            // Remove os concorrentes e seus reajustes
            _db.Concorrentes.Where(c => c.NegociacaoId == (int)key[0])
                .ToList().ForEach(c =>
                {
                    _db.Reajustes.Remove(_db.Reajustes.Find(c.ReajusteId));
                    _db.Concorrentes.Remove(c);
                });

            // Remove as rodadas
            _rodadasService.Delete(r => r.NegociacaoId == (int)key[0]);

            // Remove os reajustes
            if (negociacao.OrcadoId.HasValue)
                _reajustesService.Delete(negociacao.OrcadoId.Value);

            if (negociacao.NegociadoId.HasValue)
                _reajustesService.Delete(negociacao.NegociadoId.Value);

            _arquivosService.DeleteFiles(DependencyFileType.Negociacao, (int)key[0]);

            return base.Delete((int)key[0]);
        }

        public RodadaNegociacao AbrirRodada(int idNegociacao)
        {
            if (!Exist(idNegociacao))
                return null;

            RodadaNegociacao rodada = new RodadaNegociacao
            {
                Data = DateTime.Today,
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

        public ParcelaReajuste AdicionarParcela(ParcelaReajuste parcela)
        {
            return _parcelasService.Add(parcela);
        }

        public ParcelaReajuste RemoveParcela(int id)
        {
            return _parcelasService.Delete(id);
        }

        public Relatorio GetRelatorioFinal(int negociacaoId) {
            Negociacao negociacao = Find(negociacaoId);
            if (negociacao == null) throw new NotFoundException();

            Relatorio relatorio = _db.Relatorios.Where(r => r.NegociacaoId == negociacaoId)
                .Include(r => r.GruposPerguntas)
                    .ThenInclude(g => g.Respostas)
                .FirstOrDefault();

            relatorio.GruposPerguntas = relatorio.GruposPerguntas.OrderBy(g => g.Ordem).ToList();
            foreach (GrupoPergunta grupo in relatorio.GruposPerguntas) {
                grupo.Respostas = grupo.Respostas.OrderBy(r => r.Ordem).ToList();
            }
            return relatorio;
        }
        
        public Relatorio GenerateRelatorioFinal (int negociacaoId)
        {
            Negociacao negociacao = Find(negociacaoId);
            if (negociacao == null) throw new NotFoundException();

            Relatorio relatorio = _db.Relatorios.FirstOrDefault(r => r.NegociacaoId == negociacaoId);
            if (relatorio == null)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        relatorio = new Relatorio
                        {
                            NegociacaoId = negociacaoId,
                            Titulo = "Relatório - Acordo Coletivo Algar"
                        };
                        _db.Relatorios.Add(relatorio);
                        _db.SaveChanges();

                        List<GrupoPerguntaPadrao> gruposPerguntasPadrao = _db.GruposPerguntasPadrao
                            .Include(g => g.Perguntas).ToList();

                        foreach (GrupoPerguntaPadrao grupoPadrao in gruposPerguntasPadrao)
                        {
                            GrupoPergunta grupo = new GrupoPergunta
                            {
                                RelatorioId = relatorio.Id,
                                Ordem = grupoPadrao.Ordem,
                                Texto = grupoPadrao.Texto
                            };
                            _db.GruposPerguntas.Add(grupo);
                            _db.SaveChanges();

                            foreach (PerguntaPadrao perguntaPadrao in grupoPadrao.Perguntas)
                            {
                                RespostaRelatorio resposta = new RespostaRelatorio
                                {
                                    GrupoPerguntaId = grupo.Id,
                                    Ordem = perguntaPadrao.Ordem,
                                    Pergunta = perguntaPadrao.Texto,
                                    Resposta = RespostaPadrao(perguntaPadrao.Texto, negociacao),
                                    NumColunas = perguntaPadrao.NumColunas
                                };
                                _db.RespostasRelatorio.Add(resposta);
                            }
                            _db.SaveChanges();
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }
            return relatorio;
        }

        public void SaveRelatorio(int negociacaoId, Relatorio relatorio) {
            relatorio.NegociacaoId = negociacaoId;
            Relatorio oldRelatorio = _db.Relatorios.Find(relatorio.Id);
            if (oldRelatorio != null) {
                foreach (GrupoPergunta grupo in relatorio.GruposPerguntas) {
                    IEnumerable<RespostaRelatorio> respostas = grupo.Respostas;
                    grupo.Respostas = null;
                    GrupoPergunta oldGrupo = _db.GruposPerguntas.Find(grupo.Id);
                    if (oldGrupo == null) {
                        _db.GruposPerguntas.Add(grupo);
                        _db.SaveChanges();
                    } else {
                        _db.Entry(oldGrupo).CurrentValues.SetValues(grupo);
                    }
                    foreach (RespostaRelatorio resposta in respostas) {
                        resposta.GrupoPerguntaId = grupo.Id;
                        RespostaRelatorio oldResposta = _db.RespostasRelatorio.Find(resposta.Id);
                        if (oldResposta == null) {
                            _db.RespostasRelatorio.Add(resposta);
                        } else {
                            _db.Entry(oldResposta).CurrentValues.SetValues(resposta);
                        }
                    }
                }
                relatorio.GruposPerguntas = null;
                _db.Entry(oldRelatorio).CurrentValues.SetValues(relatorio);
                _db.SaveChanges();
            }
        }

        private string RespostaPadrao(string pergunta, Negociacao negociacao)
        {
            switch (pergunta.Trim().ToLower())
            {
                case "nome da empresa":
                    return negociacao.Empresa?.Nome ?? "";
                case "cnpj":
                    return negociacao.Empresa?.Cnpj;
                case "uf":
                    return negociacao.Empresa.Endereco?.UF ?? "";
                case "nome sindicato laboral":
                    return negociacao.SindicatoLaboral?.Nome ?? "";
                case "nome sindicato patronal":
                    return negociacao.SindicatoPatronal?.Nome ?? "";
                case "instrumento coletivo (ACT/CCT)":
                    return negociacao.SindicatoLaboral == null ? "" : (negociacao.SindicatoLaboral.Cct_act == CCT_ACT.ACT ? "ACT" : "CCT");
                case "data base":
                    return negociacao.SindicatoLaboral == null ? "" :
                        (negociacao.SindicatoLaboral.Database == Mes.Marco ? "Março" : negociacao.SindicatoLaboral.Database.ToString());
                default:
                    return "";
            }
        }

        public void DeleteRespostaRelatorio(int negociacaoId, int respostaId) {
            RespostaRelatorio resposta = _db.RespostasRelatorio.Find(respostaId);
            if (resposta == null) throw new NotFoundException();
            _db.Entry(resposta).Reference(r => r.GrupoPergunta).Load();
            _db.Entry(resposta.GrupoPergunta).Reference(g => g.Relatorio).Load();
            if (resposta.GrupoPergunta.Relatorio.NegociacaoId != negociacaoId) throw new Exception($"A pergunta de ID {respostaId} não pertence ao relatório da negociação {negociacaoId}!");
            _db.RespostasRelatorio.Remove(resposta);
            _db.SaveChanges();
        }

        public void DeleteGrupoRelatorio(int negociacaoId, int grupoId) {
            GrupoPergunta grupo = _db.GruposPerguntas.Find(grupoId);
            if (grupo == null) throw new NotFoundException();
            _db.Entry(grupo).Reference(r => r.Relatorio).Load();
            _db.Entry(grupo).Collection(r => r.Respostas).Load();
            if (grupo.Relatorio.NegociacaoId != negociacaoId) throw new Exception($"O grupo de ID {grupoId} não pertence ao relatório da negociação {negociacaoId}!");
            foreach (RespostaRelatorio resposta in grupo.Respostas) {
                _db.RespostasRelatorio.Remove(resposta);
            }
            _db.GruposPerguntas.Remove(grupo);
            _db.SaveChanges();
        }

    }
}
