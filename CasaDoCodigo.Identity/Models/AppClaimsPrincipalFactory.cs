using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Models
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
                    new Claim("name", user.Nome)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Telefone))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("phone", user.Telefone)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Endereco))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("address", user.Endereco)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Complemento))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("address_details", user.Complemento)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Bairro))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("neighborhood", user.Bairro)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Municipio))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("city", user.Municipio)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.UF))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("state", user.UF)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.CEP))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("zip_code", user.CEP)
                });
            }
            return principal;
        }
    }
}
