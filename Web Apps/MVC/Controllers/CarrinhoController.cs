using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.Models;
using MVC.Models.ViewModels;
using MVC.Services;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    [Authorize]
    public class CarrinhoController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;
        private readonly ICatalogoService catalogoService;
        private readonly ICarrinhoService carrinhoService;

        public CarrinhoController(
            IHttpContextAccessor contextAccessor,
            IIdentityParser<ApplicationUser> appUserParser,
            ILogger<CarrinhoController> logger,
            ICatalogoService catalogoService,
            ICarrinhoService carrinhoService,
            IUserRedisRepository repository)
            : base(logger, repository)
        {
            this.appUserParser = appUserParser;
            this.catalogoService = catalogoService;
            this.carrinhoService = carrinhoService;
        }

        public async Task<IActionResult> Index(string codigo = null)
        {
            try
            {
                string idUsuario = GetUserId();

                CarrinhoCliente carrinho;
                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    var produto = await catalogoService.GetProduto(codigo);
                    if (produto == null)
                    {
                        return RedirectToAction("ProdutoNaoEncontrado", "Carrinho", codigo);
                    }

                    ItemCarrinho itemCarrinho = new ItemCarrinho(produto.Codigo, produto.Codigo, produto.Nome, produto.Preco, 1, produto.UrlImagem);
                    carrinho = await carrinhoService.AddItem(idUsuario, itemCarrinho);
                }
                else
                {
                    carrinho = await carrinhoService.GetCarrinho(idUsuario);
                }
                await CheckUserCounterData();
                return View(carrinho);
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

        async Task<CarrinhoCliente> AdicionarProdutoAsync(string codigo = null)
        {
            string idUsuario = GetUserId();
            CarrinhoCliente carrinho;
            if (!string.IsNullOrWhiteSpace(codigo))
            {
                var product = await catalogoService.GetProduto(codigo);
                if (product == null)
                {
                    return null;
                }

                ItemCarrinho itemCarrinho =
                    new ItemCarrinho(product.Codigo
                    , product.Codigo
                    , product.Nome
                    , product.Preco
                    , 1
                    , product.UrlImagem);
                carrinho = await carrinhoService.AddItem(idUsuario, itemCarrinho);
                await CheckUserCounterData();
            }
            else
            {
                carrinho = await carrinhoService.GetCarrinho(idUsuario);
            }
            return carrinho;
        }

        public IActionResult ProdutoNaoEncontrado(string codigo)
        {
            return View(codigo);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantidades, string action)
        {
            try
            {
                var usuario = appUserParser.Parse(HttpContext.User);
                var carrinho = await carrinhoService.DefinirQuantidades(usuario, quantidades);
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(carrinhoService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateQuantidade([FromBody]UpdateQuantidadeInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UpdateQuantidadeOutput value = await carrinhoService.UpdateItem(GetUserId(), input);
            if (value == null)
            {
                return NotFound(input);
            }

            return base.Ok(value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Cadastro cadastro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var viewModel = new CadastroViewModel(cadastro);
                    await carrinhoService.Checkout(GetUserId(), viewModel);
                    return RedirectToAction("Checkout");
                }
                return RedirectToAction("Index", "Cadastro");
            }
            catch (BrokenCircuitException e)
            {
                logger.LogError(e, e.Message);
                HandleBrokenCircuitException(carrinhoService);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            try
            {
                string idUsuario = GetUserId();

                var usuario = appUserParser.Parse(HttpContext.User);
                await CheckUserCounterData();
                return View(new PedidoConfirmado(usuario.Email));
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                HandleException();
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AdicionarAoCarrinho([FromBody]string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return NotFound(codigo);
            }

            CarrinhoCliente carrinho = await AdicionarProdutoAsync(codigo);

            return base.Ok(carrinho);
        }
    }
}