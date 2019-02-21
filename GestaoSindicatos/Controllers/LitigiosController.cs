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
    public class LitigiosController : ControllerBase
    {
        private readonly LitigiosService _service;
        private readonly ArquivosService _arquivosService;

        public LitigiosController(LitigiosService service, ArquivosService arquivosService)
        {
            _service = service;
            _arquivosService = arquivosService;
        }

        public ActionResult<List<Litigio>> Get(int? empresaId = null, int? laboralId = null, int? patronalId = null, int? ano = null, DateTime? de = null, DateTime? ate = null)
        {
            return _service.Query(n => 
                FilterQuery.And(
                    new Tuple<object, object>(n.LaboralId, laboralId),
                    new Tuple<object, object>(n.PatronalId, patronalId),
                    new Tuple<object, object>(n.EmpresaId, empresaId), 
                    new Tuple<object, object>(n.Data.Year, ano)), User)
                .Where(l => (!de.HasValue || !ate.HasValue || (l.Data >= de.Value && l.Data <= ate.Value)))
                .Include(e => e.Empresa).Include(e => e.Laboral).Include(e => e.Patronal)
                .Include(l => l.Itens).ThenInclude(i => i.PlanoAcao)
                .OrderByDescending(l => l.Data)
                .Select(litigio => _service.UpdateStatus(litigio)).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Litigio> Get(int id)
        {
            try
            {
                Litigio litigio = _service.Find(id);
                if (litigio == null) return NotFound("Litigio não encontrado!");
                return litigio;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Litigio> Post(Litigio litigio)
        {
            try
            {
                litigio = _service.Add(litigio);
                return litigio;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult Put(int id, Litigio litigio)
        {
            try
            {
                return Ok(_service.Update(litigio, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Litigio não encontrado!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<Litigio> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Litigio não encontrado!");
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
                return Ok(_arquivosService.GetFiles(DependencyFileType.Litigio, id));
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
                _arquivosService.SaveFiles(DependencyFileType.Litigio, id, Request.Form.Files);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}