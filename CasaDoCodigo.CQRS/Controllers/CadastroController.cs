using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CadastroController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;

        public IConfiguration Configuration { get; }

        public CadastroController(
            IHttpContextAccessor contextAccessor,
            IIdentityParser<ApplicationUser> appUserParser
            )
            : base(contextAccessor)
        {
            this.appUserParser = appUserParser;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuario = appUserParser.Parse(HttpContext.User);
                CadastroViewModel cadastro
                    = new CadastroViewModel()
                    {
                        Bairro = usuario.Bairro,
                        CEP = usuario.CEP,
                        Complemento = usuario.Complemento,
                        Email = usuario.Email,
                        Endereco = usuario.Endereco,
                        Municipio = usuario.Municipio,
                        Nome = usuario.Nome,
                        Telefone = usuario.Telefone,
                        UF = usuario.UF
                    };

                return View(cadastro);
            }
            catch (BrokenCircuitException)
            {
                HandleBrokenCircuitException();
            }
            catch (Exception e)
            {
                HandleBrokenCircuitException();
            }
            return View();
        }
    }
}
