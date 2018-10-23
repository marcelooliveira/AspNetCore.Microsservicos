using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CasaDoCodigo.Identity.Models
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            if (!string.IsNullOrWhiteSpace(user.Nome))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Name, user.Nome)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Telefone))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.MobilePhone, user.Telefone)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Endereco))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.StreetAddress, user.Endereco)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Complemento))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.StreetAddress, user.Complemento)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Bairro))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.StreetAddress, user.Bairro)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Municipio))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Locality, user.Municipio)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.UF))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.StateOrProvince, user.UF)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.CEP))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.PostalCode, user.CEP)
                });
            }
            return principal;
        }
    }
}
