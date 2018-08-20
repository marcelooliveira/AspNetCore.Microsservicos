using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using CasaDoCodigo.Models;
using CasaDoCodigo.API.Areas.Identity.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : BaseApiController
    {
        public LoginController(ILogger<LoginController> logger) : base(logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="usersDAO"></param>
        /// <param name="signingConfigurations"></param>
        /// <param name="tokenConfigurations"></param>
        /// <returns></returns>
        /// <response code="400">Login inválido</response> 
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Post(
            [FromBody]User usuario,
            [FromServices]UsersDAO usersDAO,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            if (usuario == null || String.IsNullOrWhiteSpace(usuario.Id))
            {
                return BadRequest("Login inválido");
            }

            var usuarioBase = await usersDAO.Find(usuario.Id);

            if (usuarioBase == null ||
                (usuario.Id != usuarioBase.Id ||
                usuario.PasswordHash != usuarioBase.PasswordHash))
            {
                return BadRequest("Login inválido");
            }

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(usuario.Id, "Login"),
                new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Id)
                }
            );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao +
                TimeSpan.FromSeconds(tokenConfigurations.Seconds);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                SigningCredentials = signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            return Ok(token);
        }
    }
}