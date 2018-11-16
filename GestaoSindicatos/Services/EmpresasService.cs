using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using GestaoSindicatos.Exceptions;
using System.Linq.Expressions;
using System.Security.Claims;
using GestaoSindicatos.Auth;
using Microsoft.AspNetCore.Identity;

namespace GestaoSindicatos.Services
{
    public class EmpresasService : CrudService<Empresa>
    {
        private readonly Context _db;
        private readonly CrudService<ContatoEmpresa> _contatosEmpresaService;
        private readonly ContatosService _contatosService;
        private readonly CrudService<Endereco> _enderecosService;
        private readonly ArquivosService _arquivosService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpresasService(Context db,
            CrudService<ContatoEmpresa> contatosEmpresaService,
            CrudService<Endereco> enderecosService,
            ContatosService contatosService,
            ArquivosService arquivosService,
            UserManager<ApplicationUser> userManager) : base(db)
        {
            _db = db;
            _contatosEmpresaService = contatosEmpresaService;
            _contatosService = contatosService;
            _enderecosService = enderecosService;
            _arquivosService = arquivosService;
            _userManager = userManager;
        }


        private Expression<Func<Empresa, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (Empresa e) => _db.EmpresasUsuarios.Any(u => u.UserName == claims.Identity.Name && u.EmpresaId == e.Id);
        }

        public IQueryable<Empresa> Query(Expression<Func<Empresa, bool>> query, ClaimsPrincipal claims)
        {
            // Consulta somente as empresas relacionadas ao usuário, se ele não for Admin
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public IQueryable<Empresa> Query(ClaimsPrincipal claims)
        {
            // Consulta somente as empresas relacionadas ao usuário, se ele não for Admin
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(QueryUser(claims));

            return base.Query();
        }

        public IQueryable<Empresa> Query(string userName)
        {
            return _db.EmpresasUsuarios.Where(e => e.UserName == userName)
                .Include(e => e.Empresa)
                .Select(e => e.Empresa);
        }

        public override Empresa Delete(params object[] key)
        {
            if (_db.Negociacoes.Any(x => x.EmpresaId == (int)key[0]))
                throw new Exception("Existem negociações cadastradas para essa empresa!");
            if (_db.Litigios.Any(x => x.EmpresaId == (int)key[0]))
                throw new Exception("Existem litígios cadastrados para essa empresa!");

            _arquivosService.DeleteFiles(DependencyFileType.Empresa, (int)key[0]);
            _contatosEmpresaService.Query(x => x.EmpresaId == (int)key[0])
                .ToList().ForEach(x => _contatosService.Delete(x.ContatoId));
            return base.Delete(key);
        }


        public List<Contato> GetContatos(int idEmpresa)
        {
            return _contatosEmpresaService.Query()
                    .Include(c => c.Contato)
                    .Where(c => c.EmpresaId == idEmpresa)
                    .Select(c => c.Contato)
                    .ToList();
        }

        public override Empresa Find(params object[] key)
        {
            Empresa empresa = base.Find(key);
            _db.Entry(empresa).Reference(e => e.Endereco).Load();
            return empresa;
        }

        public override Empresa Update(Empresa empresa, params object[] key)
        {
            if (empresa.EnderecoId == 0 && empresa.Endereco != null && empresa.Endereco.Id != 0)
                empresa.EnderecoId = empresa.Endereco.Id;
            if (empresa.Endereco != null)
                _enderecosService.Update(empresa.Endereco, empresa.EnderecoId);
            return base.Update(empresa, key);

        }

        public Contato NovoContato(int idEmpresa, Contato contato)
        {
            Empresa empresa = Find(idEmpresa);
            if (empresa == null) throw new NotFoundException();

            contato = _contatosService.Add(contato);

            _contatosEmpresaService.Add(new ContatoEmpresa
            {
                ContatoId = contato.Id,
                EmpresaId = idEmpresa
            });

            return contato;

        }

        public Contato DeleteContato(int idEmpresa, int idContato)
        {
            if (!Exist(idEmpresa))
                throw new NotFoundException($"Empresa não encontrada! (id: {idEmpresa})");

            Contato contato = _contatosService.Find(idContato);
            if (contato == null) throw new NotFoundException($"Contato não encontrado! (id: {idContato})");

            _contatosEmpresaService.Delete(c => c.ContatoId == idContato && c.EmpresaId == idEmpresa);
            _contatosService.Delete(idContato);

            return contato;
        }

        public void AllowUser(int idEmpresa, string userName)
        {

            if (!Exist(idEmpresa))
                throw new NotFoundException("Empresa");

            var user = _userManager.FindByNameAsync(userName).Result;
            if (user == null)
                throw new NotFoundException("Usuário");

            if (_userManager.IsInRoleAsync(user, Roles.ADMIN).Result)
                throw new Exception("O usuário já é administrador!");
                
            if (_db.EmpresasUsuarios.Any(x => x.EmpresaId == idEmpresa && x.UserName == userName)) return;

            _db.EmpresasUsuarios.Add(new EmpresaUsuario
            {
                EmpresaId = idEmpresa,
                UserName = userName
            });
            _db.SaveChanges();
        }

        public void DisallowUser(int idEmpresa, string userName)
        {
            if (!Exist(idEmpresa))
                throw new NotFoundException("Empresa");

            var user = _userManager.FindByNameAsync(userName).Result;
            if (user == null)
                throw new NotFoundException("Usuário");

            if (_userManager.IsInRoleAsync(user, Roles.ADMIN).Result)
                throw new Exception("O usuário já é administrador!");

            if (!_db.EmpresasUsuarios.Any(x => x.EmpresaId == idEmpresa && x.UserName == userName)) return;

            _db.EmpresasUsuarios.Where(x => x.EmpresaId == idEmpresa && x.UserName == userName)
                .ToList().ForEach(x => _db.EmpresasUsuarios.Remove(x));
            _db.SaveChanges();

        }
    }
}
