using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using GestaoSindicatos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GestaoSindicatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly UsuariosService _usuariosService;

        public LoginController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            UsuariosService usuariosService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _usuariosService = usuariosService;
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Usuario usuario)
        {
            ApplicationUser appUser = null;
            bool credenciaisValidas = false;
            if (usuario != null && !string.IsNullOrWhiteSpace(usuario.Login))
            {
                // Verifica a existência do usuário nas tabelas do
                // ASP.NET Core Identity
                appUser = _userManager
                    .FindByNameAsync(usuario.Login).Result;
                
                if (appUser != null)
                {
                    // Efetua o login com base no Id do usuário e sua senha
                    var resultadoLogin = _signInManager
                        .CheckPasswordSignInAsync(appUser, usuario.Senha, false)
                        .Result;

                    credenciaisValidas = resultadoLogin.Succeeded;
                }
            }

            if (credenciaisValidas)
            {
                // Cria o Claims, e ADICIONA OS ROLES (IMPORTANTE!!!!!!!!!!!!!!!!!)
                Claim[] claimsRoles = _userManager.GetRolesAsync(appUser)
                    .Result.Select(x => new Claim(ClaimTypes.Role, x)).ToArray();
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(usuario.Login, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Login)
                    }.Concat(claimsRoles)
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = _tokenConfigurations.Issuer,
                    Audience = _tokenConfigurations.Audience,
                    SigningCredentials = _signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);

                return Ok(new AuthInfo
                {
                    Authenticated = true,
                    Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserName = appUser.UserName,
                    Roles = claimsRoles.Select(x => x.Value).ToArray(),
                    AccessToken = token,
                    Message = "OK"
                });
            }
            else
            {
                return Ok(new AuthInfo
                {
                    Authenticated = false,
                    Message = "Credenciais inválidas!"
                });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("registro")]
        public ActionResult Registro(Usuario user)
        {
            try
            {
                _usuariosService.CreateUser(user, user.Senha, Roles.PADRAO);

                return Ok();
            } catch (Exception e)
            {
                if (e.Message == PasswordErros.NonAlphanumeric)
                {
                    return BadRequest("A senha deve ter pelo menos um caractere especial!");
                } else if (e.Message == PasswordErros.Digit)
                {
                    return BadRequest("A senha deve ter pelo menos um dígito numérico!");
                } else if (e.Message == PasswordErros.Upper)
                {
                    return BadRequest("A senha deve ter pelo menos um caractere maiúsculo!");
                } else if (e.Message == PasswordErros.Lower)
                {
                    return BadRequest("A senha deve ter pelo menos um caractere minúsculo!");
                } else if (e.Message == PasswordErros.TooShort)
                {
                    return BadRequest("A senha deve ter pelo menos 8 caracteres!");
                }
                return BadRequest(e.Message);
            }
        }

        
    }
}