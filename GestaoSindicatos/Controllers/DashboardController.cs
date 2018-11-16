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

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;
        
        public DashboardController(DashboardService service)
        {
            _service = service;
        }

        [HttpGet("plr/massasalarial/{ano}")]
        public ActionResult<List<PieChartData>> GetPLRMassaSalarial(int ano)
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
        public ActionResult<List<PieChartData>> GetPLRQtdaTrabalhadores(int ano)
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
        public ActionResult<List<PieChartData>> GetACTMassaSalarial(int ano)
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
        public ActionResult<List<PieChartData>> GetACTQtdaTrabalhadores(int ano)
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
    }
}