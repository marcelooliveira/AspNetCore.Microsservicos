using Identity.API.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API
{
    public class ProfileService : IProfileService
    {
        protected readonly IHttpContextAccessor contextAccessor;
        protected readonly IServiceProvider serviceProvider;

        public ProfileService(IHttpContextAccessor contextAccessor, IServiceProvider serviceProvider)
        {
            this.contextAccessor = contextAccessor;
            this.serviceProvider = serviceProvider;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.FindFirst("sub").Value;
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userMgr.FindByIdAsync(userId);
                var claims = await userMgr.GetClaimsAsync(user);
                context.IssuedClaims.Add(new Claim("name", user.Nome ?? ""));
                context.IssuedClaims.Add(new Claim("email", user.Email ?? ""));
                context.IssuedClaims.Add(new Claim("phone", user.Telefone ?? ""));
                context.IssuedClaims.Add(new Claim("address", user.Endereco ?? ""));
                context.IssuedClaims.Add(new Claim("address_details", user.Complemento ?? ""));
                context.IssuedClaims.Add(new Claim("neighborhood", user.Bairro ?? ""));
                context.IssuedClaims.Add(new Claim("city", user.Municipio ?? ""));
                context.IssuedClaims.Add(new Claim("state", user.UF ?? ""));
                context.IssuedClaims.Add(new Claim("zip_code", user.CEP ?? ""));
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(true);
        }
    }
}
