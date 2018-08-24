using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CasaDoCodigo.Models
{
    public class UsuarioInput
    {
        public string UsuarioId { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
    }

    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string Key { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }

    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations()
        {
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            SigningCredentials = new SigningCredentials(
                Key, SecurityAlgorithms.RsaSha256Signature);
        }
    }
}