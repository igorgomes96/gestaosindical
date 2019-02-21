using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GestaoSindicatos.Auth;
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
    public class ArquivosController : ControllerBase
    {
        private readonly ArquivosService _arquivosService;

        public ArquivosController(ArquivosService arquivosRepositoy)
        {
            _arquivosService = arquivosRepositoy;
        }


        [HttpGet("{id}/download")]
        public FileResult Donwload(long id)
        {
            Arquivo arquivo = _arquivosService.Find(id);
            Stream stream = new FileStream(
                Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo.Path),
                FileMode.Open);
            return File(stream, arquivo.ContentType);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Delete(long id)
        {
            _arquivosService.Delete(id);
            return Ok();
        }
    }
}