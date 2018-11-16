using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PesquisaController : ControllerBase
    {
        private readonly PesquisaService _pesquisaService;

        public PesquisaController(PesquisaService pesquisaService)
        {
            _pesquisaService = pesquisaService;
        }

        public ActionResult<List<PesquisaResult>> Get(string nome = null)
        {
            if (nome == null)
                return null;
            try
            {
                return _pesquisaService.Pesquisa(nome);
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}