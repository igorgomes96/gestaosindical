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
    public class SindicatosPatronaisService : CrudService<SindicatoPatronal>
    {
        private readonly Context _db;
        private readonly CrudService<ContatoSindicatoPatronal> _contatosSindService;
        private readonly ContatosService _contatosService;
        private readonly ArquivosService _arquivosService;

        public SindicatosPatronaisService(Context db,
            CrudService<ContatoSindicatoPatronal> contatosSindService,
            ContatosService contatosService,
            ArquivosService arquivosService) : base(db)
        {
            _db = db;
            _contatosSindService = contatosSindService;
            _contatosService = contatosService;
            _arquivosService = arquivosService;
        }

        private Expression<Func<SindicatoPatronal, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (SindicatoPatronal s) => _db.EmpresasUsuarios
                .Where(e => e.UserName == claims.Identity.Name)
                .Include(e => e.Empresa)
                .Any(e => e.Empresa.SindicatoPatronalId == s.Id);
        }

        public IQueryable<SindicatoPatronal> Query(Expression<Func<SindicatoPatronal, bool>> query, ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public IQueryable<SindicatoPatronal> Query(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query().Where(QueryUser(claims));

            return base.Query();
        }


        public List<Contato> GetContatos(int idSindicato)
        {
            return _contatosSindService.Query()
                    .Include(c => c.Contato)
                    .Where(c => c.SindicatoPatronalId == idSindicato)
                    .Select(c => c.Contato)
                    .ToList();
        }

        public override void Delete(Expression<Func<SindicatoPatronal, bool>> query)
        {
            Query(query).ToList().ForEach(s => Delete(s.Id));
        }

        public override SindicatoPatronal Delete(params object[] key)
        {
            if (_db.Empresas.Any(x => x.SindicatoPatronalId == (int)key[0]))
                throw new Exception("Existem empresas relacionadas à esse sindicato!");
            if (_db.Negociacoes.Any(x => x.SindicatoPatronalId == (int)key[0]))
                throw new Exception("Existem negociações relacionadas à esse sindicato!");
            if (_db.Litigios.Any(x => x.PatronalId == (int)key[0]))
                throw new Exception("Existem litígios relacionados à esse sindicato!");
            /*if (_db.PlanosAcao.Any(x => x.PatronalId == (int)key[0]))
                throw new Exception("Existem planos de ações relacionados à esse sindicato!");*/

            _arquivosService.DeleteFiles(DependencyFileType.SindicatoPatronal, (int)key[0]);
            _contatosSindService.Query(x => x.SindicatoPatronalId == (int)key[0])
                .ToList().ForEach(x => _contatosService.Delete(x.ContatoId));
            return base.Delete(key);
        }

        public Contato NovoContato(int idSindicato, Contato contato)
        {
            SindicatoPatronal sindicato = Find(idSindicato);
            if (sindicato == null) throw new NotFoundException();

            contato = _contatosService.Add(contato);

            _contatosSindService.Add(new ContatoSindicatoPatronal
            {
                ContatoId = contato.Id,
                SindicatoPatronalId = idSindicato
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
