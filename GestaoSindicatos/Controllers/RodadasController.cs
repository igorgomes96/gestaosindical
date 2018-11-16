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

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class RodadasController : ControllerBase
    {
        private readonly RodadasService _service;
        private readonly ArquivosService _arquivosService;

        public RodadasController(Context db, RodadasService service, ArquivosService arquivosService)
        {
            _service = service;
            _arquivosService = arquivosService;
        }


        [HttpGet("{id}")]
        public ActionResult<RodadaNegociacao> Get(int id)
        {
            try
            {
                RodadaNegociacao rodada = _service.Find(id);
                if (rodada == null) return NotFound("Rodada de Negociação não encontrada!");
                return rodada;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(int id, RodadaNegociacao rodada)
        {
            try
            {
                return Ok(_service.Update(rodada, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Rodada de Negociação não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<RodadaNegociacao> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Rodada de Negociação não encontrada!");
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
                return Ok(_arquivosService.GetFiles(DependencyFileType.RodadaNegociacao, id));
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
                _arquivosService.SaveFiles(DependencyFileType.RodadaNegociacao, id, Request.Form.Files);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}