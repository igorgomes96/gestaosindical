using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestaoSindicatos.Exceptions;
using Microsoft.AspNetCore.Authorization;
using GestaoSindicatos.Auth;

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class PlanosAcaoController : ControllerBase
    {
        private readonly PlanosAcaoService _service;
        private readonly ArquivosService _arquivosService;
        private readonly EmpresasService _empresasService;

        public PlanosAcaoController(PlanosAcaoService service, ArquivosService arquivosService, EmpresasService empresasService)
        {
            _service = service;
            _arquivosService = arquivosService;
            _empresasService = empresasService;
        }

        [HttpGet("{id}")]
        public ActionResult<PlanoAcao> Get(int id)
        {
            try
            {
                PlanoAcao planoAcao = _service.Find(id);
                if (planoAcao == null) return NotFound("Plano de Ação não encontrado!");
                return planoAcao;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<PlanoAcao> Post(PlanoAcao planoAcao)
        {
            try
            {
                planoAcao = _service.Add(planoAcao);
                return planoAcao;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(int id, PlanoAcao planoAcao)
        {
            try
            {
                return Ok(_service.Update(planoAcao, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Plano de Ação não encontrado!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<PlanoAcao> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Plano de Ação não encontrado!");
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
                return Ok(_arquivosService.GetFiles(DependencyFileType.PlanoAcao, id));
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
                _arquivosService.SaveFiles(DependencyFileType.PlanoAcao, id, Request.Form.Files);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}