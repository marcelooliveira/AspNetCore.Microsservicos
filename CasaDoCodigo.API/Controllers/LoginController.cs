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
using Microsoft.AspNetCore.Identity;
using CasaDoCodigo.API.Areas.Identity.Data;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : BaseApiController
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<CasaDoCodigoAPIUser> _hasher;
        private readonly UserManager<CasaDoCodigoAPIUser> _userMgr;

        public LoginController(ILogger<LoginController> logger,
            IConfiguration configuration,
            IPasswordHasher<CasaDoCodigoAPIUser> hasher,
            UserManager<CasaDoCodigoAPIUser> userMgr) : base(logger)
        {
            _config = configuration;
            _hasher = hasher;
            _userMgr = userMgr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="usersDAO"></param>
        /// <returns></returns>
        /// <response code="400">Login inválido</response> 
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Post(
            [FromBody]UsuarioInput input,
            [FromServices]UsersDAO usersDAO)
        {
            if (input == null || String.IsNullOrWhiteSpace(input.UsuarioId))
            {
                return BadRequest("Login inválido");
            }

            var user = await _userMgr.FindByIdAsync(input.UsuarioId);
            if (user == null)
            {
                return BadRequest("Login inválido");
            }

            if (_hasher.VerifyHashedPassword(user, user.PasswordHash, input.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Login inválido");
            }
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, input.UsuarioId)
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