using GestaoSindicatos.Auth;
using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class SindicatosLaboraisService : CrudService<SindicatoLaboral>
    {
        private readonly Context _db;
        private readonly CrudService<ContatoSindicatoLaboral> _contatosSindService;
        private readonly ContatosService _contatosService;
        private readonly ArquivosService _arquivosService;
        public SindicatosLaboraisService(Context db,
            CrudService<ContatoSindicatoLaboral> contatosSindService,
            ContatosService contatosService,
            ArquivosService arquivosService) : base(db)
        {
            _db = db;
            _contatosSindService = contatosSindService;
            _contatosService = contatosService;
            _arquivosService = arquivosService;
        }

        private Expression<Func<SindicatoLaboral, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (SindicatoLaboral s) => _db.EmpresasUsuarios
                .Where(e => e.UserName == claims.Identity.Name)
                .Include(e => e.Empresa)
                .Any(e => e.Empresa.SindicatoLaboralId == s.Id);
        }

        public IQueryable<SindicatoLaboral> Query(Expression<Func<SindicatoLaboral, bool>> query, ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public IQueryable<SindicatoLaboral> Query(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query().Where(QueryUser(claims));

            return base.Query();
        }

        public override SindicatoLaboral Delete(params object[] key)
        {
            if (_db.Empresas.Any(x => x.SindicatoLaboralId == (int)key[0]))
                throw new Exception("Existem empresas relacionadas à esse sindicato!");
            if (_db.Negociacoes.Any(x => x.SindicatoLaboralId == (int)key[0]))
                throw new Exception("Existem negociações relacionadas à esse sindicato!");
            if (_db.Litigios.Any(x => x.LaboralId == (int)key[0]))
                throw new Exception("Existem litígios relacionados à esse sindicato!");
            if (_db.PlanosAcao.Any(x => x.LaboralId == (int)key[0]))
                throw new Exception("Existem planos de ações relacionados à esse sindicato!");

            _arquivosService.DeleteFiles(DependencyFileType.SindicatoLaboral, (int)key[0]);
            _contatosSindService.Query(x => x.SindicatoLaboralId == (int)key[0])
                .ToList().ForEach(x => _contatosService.Delete(x.ContatoId));
            return base.Delete(key);
        }

        public List<Contato> GetContatos(int idSindicato)
        {
            return _contatosSindService.Query()
                    .Include(c => c.Contato)
                    .Where(c => c.SindicatoLaboralId == idSindicato)
                    .Select(c => c.Contato)
                    .ToList();
        }
        public Contato NovoContato(int idSindicato, Contato contato)
        {
            SindicatoLaboral sindicato = Find(idSindicato);
            if (sindicato == null) throw new NotFoundException();

            contato = _contatosService.Add(contato);

            _contatosSindService.Add(new ContatoSindicatoLaboral
            {
                ContatoId = contato.Id,
                SindicatoLaboralId = idSindicato
            });

            return contato;

        }


        public Contato DeleteContato(int idSindicato, int idContato)
        {
            if (!Exist(idSindicato))
                throw new NotFoundException($"Sindicato não encontrado! (id: {idSindicato})");

            Contato contato = _contatosService.Find(idContato);
            if (contato == null) throw new NotFoundException($"Contato não encontrado! (id: {idContato})");

            _contatosService.Delete(idContato);

            return contato;

        }
    }
    
}
