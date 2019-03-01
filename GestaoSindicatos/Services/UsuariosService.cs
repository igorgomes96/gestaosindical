using GestaoSindicatos.Auth;
using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Identity.UI.Services;
using GestaoSindicatos.Properties;
using Microsoft.Extensions.Configuration;

namespace GestaoSindicatos.Services
{
    public class UsuariosService : CrudService<Usuario>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Context _db;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public UsuariosService(UserManager<ApplicationUser> userManager, Context context,
            IEmailSender emailSender, IConfiguration configuration) : base(context)
        {
            _userManager = userManager;
            _db = context;
            _emailSender = emailSender;
            _configuration = configuration;
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

                    string emailAdmin =  _configuration["EmailSender:EmailAdmin"];
                    string link =  _configuration["URL"];

                    _emailSender.SendEmailAsync(emailAdmin, "[Gestão Sindical] Liberação de Acesso",
                        Resource.GrantAccessTemplate.Replace("@USUARIO", user.Nome)
                            .Replace("@LOGIN", user.Login)
                            .Replace("@HORA", DateTime.Now.ToString("HH:mm:ss"))
                            .Replace("@DIA", DateTime.Now.ToString("dd/MM/yyyy"))
                            .Replace("@LINK", $"{link}/usuarios/{user.Login}")).Wait();
                }
                else
                {
                    throw new Exception(resultado.Errors.First().Code);
                }
            }
            else
            {
                throw new Exception("Usuário já cadastrado!");
            }
        }

        public override Usuario Delete(params object[] key)
        {
            ApplicationUser appUser = _userManager.FindByNameAsync(key[0].ToString()).Result;

            if (appUser == null)
                throw new NotFoundException("Usuário não encontrado!");

            _db.EmpresasUsuarios.Where(e => e.UserName == appUser.UserName)
                .ToList().ForEach(x => _db.EmpresasUsuarios.Remove(x));

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

        public string GetErrorAuthMessage(string messageError)
        {
            if (messageError == PasswordErros.NonAlphanumeric)
            {
                return "A senha deve ter pelo menos um caractere especial!";
            }
            else if (messageError == PasswordErros.Digit)
            {
                return "A senha deve ter pelo menos um dígito numérico!";
            }
            else if (messageError == PasswordErros.Upper)
            {
                return "A senha deve ter pelo menos um caractere maiúsculo!";
            }
            else if (messageError == PasswordErros.Lower)
            {
                return "A senha deve ter pelo menos um caractere minúsculo!";
            }
            else if (messageError == PasswordErros.TooShort)
            {
                return "A senha deve ter pelo menos 8 caracteres!";
            }
            else if (messageError == PasswordErros.PasswordIncorrect)
            {
                return "Senha incorreta!";
            }
            else if (messageError == PasswordErros.InvalidToken)
            {
                return "Código de Verificação Expirado! Solicite o envio novamente!";
            }
            return messageError;
        }

        public void SendRecoveryCode(string userName)
        {

            var appUser = _userManager.FindByNameAsync(userName).Result;
            if (appUser == null)
                throw new NotFoundException();

            string code = _userManager.GeneratePasswordResetTokenAsync(appUser).Result;
            _emailSender.SendEmailAsync(appUser.UserName, "[Gestão Sindical] Recuperação de Senha",
                Resource.RecoveryPasswordTemplate.Replace("@CODIGO", code)).Wait();
        }

        public void ChangePassword(Usuario user)
        {
            var appUser = _userManager.FindByNameAsync(user.Login).Result;
            if (appUser == null)
                throw new NotFoundException();

            var resultado = _userManager.ResetPasswordAsync(appUser, user.CodigoRecuperacao, user.Senha).Result;

            if (!resultado.Succeeded)
            {
                throw new Exception(resultado.Errors.First().Code);
            }

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
