using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class ArquivosController : ControllerBase
    {
        private readonly ArquivosRepository _arquivosRepository;

        public ArquivosController(ArquivosRepository arquivosRepositoy)
        {
            _arquivosRepository = arquivosRepositoy;
        }


        [HttpGet("{id}/download")]
        public FileResult Donwload(string id)
        {
            Arquivo arquivo = _arquivosRepository.Find(new ObjectId(id));
            Stream stream = new MemoryStream(arquivo.Content);
            return File(stream, arquivo.ContentType);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Delete(string id)
        {
            _arquivosRepository.Delete(new ObjectId(id));
            return Ok();
        }
    }
}