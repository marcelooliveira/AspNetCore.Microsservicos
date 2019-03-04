using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.SignalR;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CatalogoController : BaseController
    {
        private readonly ICatalogoService catalogoService;

        public CatalogoController
            (ICatalogoService catalogoService,
            ILogger<CatalogoController> logger,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.catalogoService = catalogoService;
        }

        public async Task<IActionResult> Index()
        {
            await CheckUserNotificationCount();

            try
            {
                var produtos = await catalogoService.GetProdutos();
                var resultado = new BuscaProdutosViewModel(produtos, "");
                return base.View(resultado);
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(catalogoService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }

            return View();
        }

        public async Task<IActionResult> BuscaProdutos(string pesquisa)
        {
            await CheckUserNotificationCount();

            try
            {
                var produtos = await catalogoService.BuscaProdutos(pesquisa);
                var resultado = new BuscaProdutosViewModel(produtos, pesquisa);
                return View("Index", resultado);
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(catalogoService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }

            return View("Index");
        }

    }
}

