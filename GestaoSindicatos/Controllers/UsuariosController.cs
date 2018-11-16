using System;
using System.Collections.Generic;
using System.Linq;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosService _service;

        public UsuariosController(UsuariosService service)
        {
            _service = service;
        }

        public ActionResult<List<Usuario>> Get()
        {
            return _service.Query().Where(u => u.Login != User.Identity.Name).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Usuario> Get(string id)
        {
            try
            {
                Usuario usuario = _service.Find(id);
                if (usuario == null) return NotFound("Usuario não encontrado!");
                return usuario;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("{userName}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(string userName, Usuario usuario)
        {
            try
            {
                return Ok(_service.Update(usuario, userName));
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
        public ActionResult<Usuario> Delete(string id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Usuario não encontrado!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}