using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Auth
{
    public static class PasswordErros
    {
        public const string NonAlphanumeric = "PasswordRequiresNonAlphanumeric";
        public const string Digit = "PasswordRequiresDigit";
        public const string Upper = "PasswordRequiresUpper";
        public const string Lower = "PasswordRequiresLower";
        public const string TooShort = "PasswordTooShort";
    }

    public class AuthInfo
    {
        public bool Authenticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string UserName { get; set; }
        public string[] Roles { get; set; }
        public string AccessToken { get; set; }
        public string Message { get; set; }
    }

    public static class Roles
    {
        public const string ADMIN = "Administrador";
        public const string PADRAO = "Usuário Padrão";
    }

    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }
}
