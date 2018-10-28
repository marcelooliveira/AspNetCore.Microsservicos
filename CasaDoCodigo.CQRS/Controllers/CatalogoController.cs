using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CatalogoController : BaseController
    {
        private readonly ICatalogoService catalogoService;

        public CatalogoController(ICatalogoService catalogoService,
            IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.catalogoService = catalogoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await catalogoService.GetProdutos());
            }
            catch (BrokenCircuitException e)
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
