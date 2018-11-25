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
    public class SindicatosLaboraisController : ControllerBase
    {
        private readonly SindicatosLaboraisService _service;
        private readonly ArquivosService _arquivosService;

        public SindicatosLaboraisController(SindicatosLaboraisService service,
            ArquivosService arquivosService)
        {
            _service = service;
            _arquivosService = arquivosService;
        }


        public ActionResult<List<SindicatoLaboral>> Get(string filter = null, int count = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                    return _service.Query(User).OrderBy(s => s.Nome).ToList();
                else
                    return _service.Query(s => s.Nome.Contains(filter, StringComparison.CurrentCultureIgnoreCase), User)
                        .OrderBy(s => s.Nome).Take(count).ToList();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("{id}")]
        public ActionResult<SindicatoLaboral> Get(int id)
        {
            try
            {
                SindicatoLaboral sindicato = _service.Find(id);
                if (sindicato == null) return NotFound("Sindicato não encontrado!");
                return sindicato;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

       

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<SindicatoLaboral> Post(SindicatoLaboral sindicato)
        {
            try
            {
                sindicato = _service.Add(sindicato);
                return sindicato;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(int id, SindicatoLaboral sindicato)
        {
            try
            {
                return Ok(_service.Update(sindicato, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Sindicato não encontrado!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<SindicatoLaboral> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Sindicato não encontrado!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("list")]
        public ActionResult<List<string>> GetList(string q = null) => 
            UtilService.ListProperty(_service.Query(), s => s.Nome, q).ToList();


        [HttpPost("{id}/contatos")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Contato> PostNovoContato(int id, Contato novo)
        {
            try
            {
                return _service.NovoContato(id, novo);
            }
            catch (NotFoundException)
            {
                return NotFound($"Sindicato não encontrado! (id: {id})");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}/contatos/{idContato}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Contato> DeleteContato(int id, int idContato)
        {
            try
            {
                return _service.DeleteContato(id, idContato);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
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
                return Ok(_arquivosService.GetFiles(DependencyFileType.SindicatoLaboral, id));
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
                _arquivosService.SaveFiles(DependencyFileType.SindicatoLaboral, id, Request.Form.Files);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}/contatos")]
        public ActionResult<List<Contato>> GetContatos(int id)
        {
            try
            {
                return _service.GetContatos(id);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}