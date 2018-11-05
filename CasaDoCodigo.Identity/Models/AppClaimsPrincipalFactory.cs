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
            
            ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                new Claim("name", user.Nome ?? string.Empty),
                new Claim("email", user.Email ?? string.Empty),
                new Claim("phone", user.Telefone ?? string.Empty),
                new Claim("address", user.Endereco ?? string.Empty),
                new Claim("address_details", user.Complemento ?? string.Empty),
                new Claim("neighborhood", user.Bairro ?? string.Empty),
                new Claim("city", user.Municipio ?? string.Empty),
                new Claim("state", user.UF ?? string.Empty),
                new Claim("zip_code", user.CEP ?? string.Empty)

            });
            return principal;
        }
    }
}
