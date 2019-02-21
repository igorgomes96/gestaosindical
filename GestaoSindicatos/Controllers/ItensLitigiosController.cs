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
    [Route("api/litigios/itens")]
    [ApiController]
    [Authorize("Bearer")]
    public class ItensLitigiosController : ControllerBase
    {
        private readonly ItensLitigiosService _service;
        private readonly ArquivosService _arquivosService;

        public ItensLitigiosController(Context db, ItensLitigiosService service, ArquivosService arquivosService)
        {
            _service = service;
            _arquivosService = arquivosService;
        }


        [HttpGet("{id}")]
        public ActionResult<ItemLitigio> Get(int id)
        {
            try
            {
                ItemLitigio item = _service.Find(id);
                if (item == null) return NotFound("Item não encontrado!");
                return item;
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<ItemLitigio> Post(ItemLitigio item)
        {
            try
            {
                item = _service.Add(item);
                return item;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<ItemLitigio> Put(int id, ItemLitigio item)
        {
            try
            {
                return Ok(_service.Update(item, id));
            }
            catch (NotFoundException)
            {
                return NotFound("Item não encontrado!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public ActionResult<ItemLitigio> Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Item não encontrado!");
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
                return Ok(_arquivosService.GetFiles(DependencyFileType.ItemLitigio, id));
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
                _arquivosService.SaveFiles(DependencyFileType.ItemLitigio, id, Request.Form.Files);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}