using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class NegociacoesController : ControllerBase
    {
        private readonly NegociacoesService _service;
        private readonly ArquivosService _arquivosService;
        private readonly RodadasService _rodadasService;
        private readonly ConcorrentesService _concorrentesService;

        public NegociacoesController(NegociacoesService service, ArquivosService arquivosService,
            RodadasService rodadasService, ConcorrentesService concorrentesService)
        {
            _service = service;
            _arquivosService = arquivosService;
            _rodadasService = rodadasService;
            _concorrentesService = concorrentesService;
        }

        public ActionResult<List<Negociacao>> Get(int? laboralId = null, int? patronalId = null, int? empresaId = null, int? ano = null)
        {
            IQueryable<Negociacao> negociacoes = _service
                .Query(n => FilterQuery.And(
                    new Tuple<object, object>(n.SindicatoLaboralId, laboralId),
                    new Tuple<object, object>(n.SindicatoPatronalId, patronalId),
                    new Tuple<object, object>(n.EmpresaId, empresaId),
                    new Tuple<object, object>(n.Ano, ano)), User)
                .Include(n => n.Empresa)
                .Include(n => n.SindicatoLaboral)
                .Include(n => n.SindicatoPatronal);
            return negociacoes.OrderBy(n => n.Empresa.Nome).ToList();
        }

        [HttpGet("calendar/{mes}")]
        public ActionResult<List<CalendarEvent>> GetCalendar(int mes)
        {
            try
            {
                return Ok(_rodadasService.GetCalendar(User, mes));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{id}")]
        public ActionResult<Negociacao> Get(int id)
        {
            try
            {
                Negociacao negociacao = _service.Find(id);
                if (negociacao == null) return NotFound("Negociação não encontrada!");
                return negociacao;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Negociacao> Post(Negociacao negociacao)
        {
            try
            {
                negociacao = _service.Add(negociacao);
                return negociacao;
            }
            catch (Exception e)
            {
                if (_service.Count(n => n.Ano == negociacao.Ano && n.EmpresaId == negociacao.EmpresaId) > 0)
                {
                    return BadRequest($"Já existe uma negociação cadastrada para esta empresa (cód. {negociacao.EmpresaId}) para o ano de {negociacao.Ano}!");
                }
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(int id, Negociacao negociacao)
        {
            try
            {
                return Ok(_service.Update(negociacao, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Negociação não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Negociacao> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Negociação não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/rodadas")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<RodadaNegociacao> PostRodada(int id)
        {
            try
            {
                RodadaNegociacao rodada = _service.AbrirRodada(id);
                if (rodada == null)
                    return NotFound("Negociação não encontrada!");
                return Ok(rodada);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{id}/rodadas")]
        public ActionResult<List<RodadaNegociacao>> GetRodadas(int id)
        {
            try
            {
                ICollection<RodadaNegociacao> rodadas = _service.GetRodadasNegociacoes(id);
                if (rodadas == null)
                    return NotFound("Negociação não encontrada!");
                return Ok(rodadas);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }

        }
        

        [HttpGet("{id}/concorrentes")]
        public ActionResult<List<Concorrente>> GetConcorrentes(int id)
        {
            try
            {
                ICollection<Concorrente> concorrentes = _concorrentesService.Query(c => c.NegociacaoId == id)
                    .Include(x => x.Reajuste).ToList();
                return Ok(concorrentes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("{id}/concorrentes")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Concorrente> PostConcorrente(int id, Concorrente concorrente)
        {
            try
            {
                if (concorrente.NegociacaoId != 0 && concorrente.NegociacaoId != id)
                    return BadRequest("Id da negociação inválido!");
                return Ok(_concorrentesService.Add(concorrente));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{negociacaoId}/concorrentes/{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Concorrente> DeleteConcorrente(int negociacaoId, int id)
        {
            try
            {
                Negociacao n = _service.Find(negociacaoId);
                if (n == null)
                    return NotFound("Negociação não encontrada!");

                Concorrente c = _concorrentesService.Find(id);
                if (c == null)
                    return NotFound("Concorrente não encontrado!");

                if (c.NegociacaoId != negociacaoId)
                    return BadRequest("Código de negociação inválido!");

                return Ok(_concorrentesService.Delete(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}/files")]
        public ActionResult<List<Arquivo>> GetFiles(int id)
        {
            try
            {
                return Ok(_arquivosService.GetFiles(DependencyFileType.Negociacao, id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/files"), DisableRequestSizeLimit]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult UploadFiles(int id)
        {
            try
            {
                _arquivosService.SaveFiles(DependencyFileType.Negociacao, id, Request.Form.Files);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("interval")]
        public ActionResult GetIntervalYears()
        {
            try
            {
                var range = _service.RangeYears();
                return Ok(new
                {
                    floor = range.Item1,
                    ceil = range.Item2
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/parcelas")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Concorrente> PostParcela(int id, ParcelaReajuste parcela)
        {
            try
            {
                return Ok(_service.AdicionarParcela(parcela));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{negociacaoId}/parcelas/{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Concorrente> DeleteParcela(int negociacaoId, int id)
        {
            try
            {
                return Ok(_service.RemoveParcela(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{negociacaoId}/relatorio")]
        public ActionResult<Relatorio> GetRelatorioFinal(int negociacaoId)
        {
            try
            {
                return Ok(_service.GetRelatorioFinal(negociacaoId));
            }
            catch (NotFoundException)
            {
                return NotFound("Negociação não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{negociacaoId}/relatorio")]
        public ActionResult<Relatorio> PostRelatorioFinal(int negociacaoId)
        {
            try
            {
                return Ok(_service.GenerateRelatorioFinal(negociacaoId));
            }
            catch (NotFoundException)
            {
                return NotFound("Negociação não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}