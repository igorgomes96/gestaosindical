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
    public class EmpresasController : ControllerBase
    {
        private readonly EmpresasService _service;
        private readonly ArquivosService _arquivosService;

        public EmpresasController(EmpresasService service, ArquivosService arquivosService)
        {
            _service = service;
            _arquivosService = arquivosService;
        }

        public ActionResult<List<Empresa>> Get(string filter = null, int count = 10)
        {
            IQueryable<Empresa> empresas;
            if (filter == null)
                empresas = _service.Query(User).Include(e => e.Endereco);
            else
                empresas = _service.Query(e => e.Nome.Contains(filter, StringComparison.CurrentCultureIgnoreCase), User)
                    .Include(e => e.Endereco);
            return empresas.ToList();
        }

        [HttpGet]
        [Route("list")]
        public ActionResult<List<object>> GetList(string filter = null, int count = 10)
        {
            IQueryable<object> empresas;
            if (filter == null)
                empresas = _service.Query().Select(e => new { e.Id, e.Cnpj ,e.Nome});
            else
                empresas = _service.Query(e => e.Nome.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                    .Select(e => new { e.Id, e.Cnpj, e.Nome });
            return empresas.ToList();
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpGet("usuarios/{userName}")]
        public ActionResult<List<Empresa>> GetRelated(string userName)
        {
            return _service.Query(userName).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Empresa> Get(int id)
        {
            try
            {
                Empresa empresa = _service.Find(id);
                if (empresa == null) return NotFound("Empresa não encontrada!");
                return empresa;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        
        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Empresa> Post(Empresa empresa)
        {
            try
            {
                empresa = _service.Add(empresa);
                return empresa;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(int id, Empresa empresa)
        {
            try
            {
                return Ok(_service.Update(empresa, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Empresa não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Empresa> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Empresa não encontrada!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


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
                return NotFound($"Empresa não encontrada! (id: {id})");
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
                return Ok(_arquivosService.GetFiles(DependencyFileType.Empresa, id));
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
                _arquivosService.SaveFiles(DependencyFileType.Empresa, id, Request.Form.Files);
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

        [HttpPost("{id}/allowuser")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult AllowUser(int id, Usuario user)
        {
            try
            {
                _service.AllowUser(id, user.Login);
                return Ok();
            } catch (NotFoundException e)
            {
                if (e.Message == "Empresa")
                    return NotFound("Empresa não encontrada!");

                if (e.Message == "Usuário")
                    return NotFound("Usuário não encontrado!");

                return NotFound();
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/disallowuser")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult DisallowUser(int id, Usuario user)
        {
            try
            {
                _service.DisallowUser(id, user.Login);
                return Ok();
            }
            catch (NotFoundException e)
            {
                if (e.Message == "Empresa")
                    return NotFound("Empresa não encontrada!");

                if (e.Message == "Usuário")
                    return NotFound("Usuário não encontrado!");

                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}