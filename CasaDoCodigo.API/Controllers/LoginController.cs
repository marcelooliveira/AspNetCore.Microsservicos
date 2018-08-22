using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using CasaDoCodigo.Models;
using CasaDoCodigo.API.Areas.Identity.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : BaseApiController
    {
        private readonly IConfiguration _config;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration) : base(logger)
        {
            _config = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="usersDAO"></param>
        /// <returns></returns>
        /// <response code="400">Login inválido</response> 
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Post(
            [FromBody]User usuario,
            [FromServices]UsersDAO usersDAO)
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

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
            _config["Tokens:Issuer"],
            claims,
            expires: DateTime.Now.AddSeconds(int.Parse(_config["Tokens:ExpiresSecs"])),
            signingCredentials: creds);

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}