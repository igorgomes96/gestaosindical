using GestaoSindicatos.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class PesquisaService
    {
        private readonly EmpresasService _empresasService;
        private readonly SindicatosLaboraisService _laboraisService;
        private readonly SindicatosPatronaisService _patronaisService;

        public PesquisaService(EmpresasService empresasService,
            SindicatosLaboraisService laboraisService,
            SindicatosPatronaisService patronaisService)
        {
            _empresasService = empresasService;
            _laboraisService = laboraisService;
            _patronaisService = patronaisService;
        }

        public List<PesquisaResult> Pesquisa(string search, ClaimsPrincipal claims)
        {
            List<PesquisaResult> result = new List<PesquisaResult>();
            search = search.ToLower().Trim();

            List<Empresa> empresas = _empresasService.Query(e => e.Nome.ToLower().Contains(search), claims)
                .Include(e => e.Endereco)
                .OrderBy(e => e.Nome)
                .Take(3).ToList();
            List<SindicatoLaboral> laborais = _laboraisService.Query(e => e.Nome.ToLower().Contains(search), claims)
                .OrderBy(e => e.Nome)
                .Take(3).ToList();
            List<SindicatoPatronal> patronais = _patronaisService.Query(e => e.Nome.ToLower().Contains(search), claims)
                .OrderBy(e => e.Nome)
                .Take(3).ToList();

            result.AddRange(empresas.Select(e => new PesquisaResult { EntityType = EntityType.Empresa, Obj = e }));
            result.AddRange(laborais.Select(e => new PesquisaResult { EntityType = EntityType.SindicatoLaboral, Obj = e }));
            result.AddRange(patronais.Select(e => new PesquisaResult { EntityType = EntityType.SindicatoPatronal, Obj = e }));

            return result;

        }
    }
}
