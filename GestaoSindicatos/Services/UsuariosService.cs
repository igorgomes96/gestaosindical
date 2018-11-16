using GestaoSindicatos.Auth;
using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class UsuariosService: CrudService<Usuario>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Context _db;

        public UsuariosService(UserManager<ApplicationUser> userManager, Context context): base(context)
        {
            _userManager = userManager;
            _db = context;
        }

        public void CreateUser(Usuario user, string password, string initialRole = null)
        {
            if (_userManager.FindByNameAsync(user.Login).Result == null)
            {
                ApplicationUser appUser = new ApplicationUser()
                {
                    UserName = user.Login,
                    Email = user.Login,
                    EmailConfirmed = true,
                };
                var resultado = _userManager.CreateAsync(appUser, password).Result;

                if (resultado.Succeeded && !string.IsNullOrWhiteSpace(initialRole))
                {
                    _userManager.AddToRoleAsync(appUser, initialRole).Wait();
                    user.Id = _userManager.FindByNameAsync(user.Login).Result.Id;
                    user.Perfil = Roles.PADRAO;
                    Add(user);
                } else
                {
                    throw new Exception(resultado.Errors.First().Code);
                }
            } else
            {
                throw new Exception("Usuário já cadastrado!");
            }
        }

        public override Usuario Delete(params object[] key)
        {
            ApplicationUser appUser = _userManager.FindByNameAsync(key[0].ToString()).Result;

            if (appUser == null)
                throw new NotFoundException("Usuário não encontrado!");

            _userManager.DeleteAsync(appUser).Wait();

            return base.Delete(key[0].ToString());
        }

        public override Usuario Find(params object[] key)
        {
            return base.Query(u => u.Login == key[0].ToString())
                .FirstOrDefault();
        }

        public override Usuario Update(Usuario entity, params object[] key)
        {
            Usuario currentEntity = Find(key);
            if (currentEntity == null) throw new NotFoundException();
            currentEntity.Perfil = entity.Perfil;
            _db.SaveChanges();

            if (entity.Perfil == Roles.ADMIN)
                UpAdmin(entity.Login);
            else
                DownAdmin(entity.Login);

            return currentEntity;
        }

        public void UpAdmin(string userName)
        {
            var appUser = _userManager.FindByNameAsync(userName).Result;
            if (appUser == null)
                throw new NotFoundException();

            _userManager.AddToRoleAsync(appUser, Roles.ADMIN).Wait();
            if (_userManager.IsInRoleAsync(appUser, Roles.PADRAO).Result)
                _userManager.RemoveFromRoleAsync(appUser, Roles.PADRAO).Wait();
        }

        public void DownAdmin(string userName)
        {
            var appUser = _userManager.FindByNameAsync(userName).Result;
            if (appUser == null)
                throw new NotFoundException();

            _userManager.AddToRoleAsync(appUser, Roles.PADRAO).Wait();
            if (_userManager.IsInRoleAsync(appUser, Roles.ADMIN).Result)
                _userManager.RemoveFromRoleAsync(appUser, Roles.ADMIN).Wait();
            
        }

    }
}
