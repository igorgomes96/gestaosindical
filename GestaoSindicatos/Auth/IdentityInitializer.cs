using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Auth
{
    public class IdentityInitializer
    {
        private readonly AuthContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityInitializer(
            AuthContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {

            if (!_roleManager.RoleExistsAsync(Roles.ADMIN).Result)
            {
                var resultado = _roleManager.CreateAsync(
                    new IdentityRole(Roles.ADMIN)).Result;
                if (!resultado.Succeeded)
                {
                    throw new Exception(
                        $"Erro durante a criação da role {Roles.ADMIN}.");
                }
            }

            if (!_roleManager.RoleExistsAsync(Roles.PADRAO).Result)
            {
                var resultado = _roleManager.CreateAsync(new IdentityRole(Roles.PADRAO)).Result;
                if (!resultado.Succeeded)
                {
                    throw new Exception(
                        $"Erro durante a criação da role {Roles.PADRAO}.");
                }
            }

            ApplicationUser appuser = new ApplicationUser()
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true,
            };

            CreateUser(appuser, "Admin01!", Roles.ADMIN);

            //IdentityResult identityResult = _userManager.AddToRoleAsync(appuser, Roles.PADRAO).Result;

        }

        private void CreateUser(
            ApplicationUser user,
            string password,
            string initialRole = null)
        {
            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var resultado = _userManager
                    .CreateAsync(user, password).Result;

                if (resultado.Succeeded &&
                    !string.IsNullOrWhiteSpace(initialRole))
                {
                    _userManager.AddToRoleAsync(user, initialRole).Wait();
                }
            }
        }
    }
}
