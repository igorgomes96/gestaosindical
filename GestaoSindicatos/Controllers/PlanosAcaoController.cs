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

        public ActionResult<List<PlanoAcao>> Get(int? laboralId = null, int? patronalId = null, int? empresaId = null, int? ano = null, DateTime? de = null, DateTime? ate = null)
        {
            if (empresaId.HasValue)
            {
                Empresa emp = _empresasService.Find(empresaId.Value);
                if (emp != null)
                {
                    return _service.Query(n =>
                        FilterQuery.Or(
                            new Tuple<object, object>(n.LaboralId, emp.SindicatoLaboralId),
                            new Tuple<object, object>(n.PatronalId, emp.SindicatoPatronalId)), User)
                        .Where(n => (!ano.HasValue || n.Data.Year == ano.Value) && (!de.HasValue || !ate.HasValue || (n.Data >= de.Value && n.Data <= ate.Value)))
                        .Include(e => e.Laboral).Include(e => e.Patronal)
                        .OrderByDescending(p => p.Data).ToList();
                }
                else
                {
                    return new List<PlanoAcao>();
                }
            }

            IQueryable<PlanoAcao> planos = _service.Query(n =>
                FilterQuery.And(
                    new Tuple<object, object>(n.LaboralId, laboralId),
                    new Tuple<object, object>(n.PatronalId, patronalId),
                    new Tuple<object, object>(n.Data.Year, ano)), User)
                .Include(e => e.Laboral).Include(e => e.Patronal).OrderByDescending(p => p.Data);
            if (de.HasValue && ate.HasValue)
                return planos.Where(p => p.Data >= de.Value && p.Data <= ate.Value).ToList();

            return planos.ToList();
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