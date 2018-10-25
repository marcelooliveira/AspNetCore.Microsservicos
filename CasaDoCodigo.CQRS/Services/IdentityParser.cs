using CasaDoCodigo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public class IdentityParser : IIdentityParser<ApplicationUser>
    {
        public ApplicationUser Parse(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claims)
            {
                return new ApplicationUser
                {
                    Nome = claims.Claims.FirstOrDefault(x => x.Type == "name")?.Value ?? "",
                    Telefone = claims.Claims.FirstOrDefault(x => x.Type == "phone")?.Value ?? "",
                    Endereco = claims.Claims.FirstOrDefault(x => x.Type == "address")?.Value ?? "",
                    Complemento = claims.Claims.FirstOrDefault(x => x.Type == "address_details")?.Value ?? "",
                    Bairro = claims.Claims.FirstOrDefault(x => x.Type == "neighborhood")?.Value ?? "",
                    Municipio = claims.Claims.FirstOrDefault(x => x.Type == "city")?.Value ?? "",
                    UF = claims.Claims.FirstOrDefault(x => x.Type == "state")?.Value ?? "",
                    CEP = claims.Claims.FirstOrDefault(x => x.Type == "zip_code")?.Value ?? ""
                };
            }
            throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
        }
    }
}
