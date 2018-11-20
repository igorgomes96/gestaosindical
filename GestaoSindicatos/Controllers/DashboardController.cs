using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoSindicatos.Model;
using GestaoSindicatos.Model.Dashboard;
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
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;
        private readonly NegociacoesService _negociacoesService;
        
        public DashboardController(DashboardService service,
            NegociacoesService negociacoesService)
        {
            _service = service;
            _negociacoesService = negociacoesService;
        }

        [HttpGet("plr/massasalarial/{ano}")]
        public ActionResult<List<ChartData>> GetPLRMassaSalarial(int ano)
        {
            try
            {
                return _service.StatusPorMassaSalarial(User, ano, n => n.StatusPLR).ToList();
            } catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("plr/trabalhadores/{ano}")]
        public ActionResult<List<ChartData>> GetPLRQtdaTrabalhadores(int ano)
        {
            try
            {
                return _service.StatusPorQtdaTrabalhadores(User, ano, n => n.StatusPLR).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("act/massasalarial/{ano}")]
        public ActionResult<List<ChartData>> GetACTMassaSalarial(int ano)
        {
            try
            {
                return _service.StatusPorMassaSalarial(User, ano, n => n.StatusACT_CCT).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("act/trabalhadores/{ano}")]
        public ActionResult<List<ChartData>> GetACTQtdaTrabalhadores(int ano)
        {
            try
            {
                return _service.StatusPorQtdaTrabalhadores(User, ano, n => n.StatusACT_CCT).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("custosviagens/{ano}")]
        public ActionResult<List<ChartData>> GetCustosViagens(int ano)
        {
            try
            {
                return _service.CustosEmViagens(User, ano).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("proximasviagens")]
        public ActionResult GetProximasViagens()
        {
            try
            {
                return Ok(_negociacoesService.ProximasViagens(User, 5)
                    .Select(x => new
                    {
                        x.Data,
                        x.Negociacao.Empresa,
                        x.Negociacao.Empresa.Endereco.Cidade,
                        x.Negociacao.Empresa.Endereco.UF,
                        x.Negociacao.MassaSalarial,
                        x.Negociacao.QtdaTrabalhadores,
                        x.NegociacaoId
                    }).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpGet("mediareajustes/{ano}")]
        public ActionResult<List<ChartDataset>> GetMediasReajustes(int ano)
        {
            try
            {
                return _service.MediaReajustes(User, ano).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("litigios/{ano}/referente")]
        public ActionResult<List<ChartData>> GetLitigiosPorReferencia(int ano)
        {
            try
            {
                return _service.LitigiosGroup(User, ano, l => l.Referente)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("litigios/{ano}/procedimento")]
        public ActionResult<List<ChartData>> GetLitigiosPorProcedimento(int ano)
        {
            try
            {
                return _service.LitigiosGroup(User, ano, l => l.Procedimento)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("litigios/{ano}/empresa")]
        public ActionResult<List<ChartData>> GetLitigiosPorEmpresa(int ano)
        {
            try
            {
                return _service.LitigiosPorEmpresa(User, ano)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("litigios/{ano}/mes")]
        public ActionResult<List<ChartData>> GetLitigiosPorMes(int ano)
        {
            try
            {
                return _service.LitigiosMensal(User, ano)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("qtdareunioes/{ano}/laboral")]
        public ActionResult<List<ChartData>> GetQtdaReunioesPorLaboral(int ano)
        {
            try
            {
                return _service.QtdaReunioes(User, ano, r => r.Negociacao.SindicatoLaboral.Nome)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("qtdareunioes/{ano}/patronal")]
        public ActionResult<List<ChartData>> GetQtdaReunioesPorPatronal(int ano)
        {
            try
            {
                return _service.QtdaReunioes(User, ano, r => r.Negociacao.SindicatoPatronal.Nome)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("qtdareunioes/{ano}/estado")]
        public ActionResult<List<ChartData>> GetQtdaReunioesPorEstado(int ano)
        {
            try
            {
                return _service.QtdaReunioes(User, ano, r => r.Negociacao.Empresa.Endereco.UF)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("qtdareunioes/{ano}/empresa")]
        public ActionResult<List<ChartData>> GetQtdaReunioesPorEmpresa(int ano)
        {
            try
            {
                return _service.QtdaReunioes(User, ano, r => r.Negociacao.Empresa.Nome)
                    .ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("planosacao/{ano}/status")]
        public ActionResult<List<ChartData>> GetStatusPlanosAcao(int ano)
        {
            try
            {
                return _service.StatusPorPlanosAcao(User, ano).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("planosacao/{ano}/referente")]
        public ActionResult<List<ChartData>> GetPlanosAcaoReferente(int ano)
        {
            try
            {
                return _service.PlanosReferenteA(User, ano).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("planosacao/{ano}/procedencia")]
        public ActionResult<List<ChartData>> GetPlanosAcaoProcedencia(int ano)
        {
            try
            {
                return _service.PlanosProcedencia(User, ano).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("negociacoes/{empresaId}")]
        public ActionResult<List<Negociacao>> GetNegociacoes(int empresaId)
        {
            try
            {
                return _negociacoesService.Query(x => x.EmpresaId == empresaId)
                    .Include(x => x.Negociado)
                    .Include(x => x.Orcado)
                    .Include(x => x.RodadasNegociacoes)
                    .ToList();

            } catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}