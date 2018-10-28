using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CarrinhoController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;
        private readonly ICatalogoService catalogoService;
        private readonly ICarrinhoService carrinhoService;

        public CarrinhoController(IIdentityParser<ApplicationUser> appUserParser, ICatalogoService catalogoService, ICarrinhoService carrinhoService, IHttpContextAccessor contextAccessor)
            : base(contextAccessor)
        {
            this.appUserParser = appUserParser;
            this.catalogoService = catalogoService;
            this.carrinhoService = carrinhoService;
        }

        [Authorize]
        public async Task<IActionResult> Index(string codigo)
        {
            try
            {
                string idUsuario = GetUserId();
                int pedidoId = GetPedidoId() ?? 0;
                var produto = await catalogoService.GetProduto(codigo);
                ItemCarrinho itemCarrinho = new ItemCarrinho(produto.Codigo, produto.Codigo, produto.Nome, produto.Preco, 1, produto.UrlImagem);
                var carrinho = await carrinhoService.AddItem(idUsuario, itemCarrinho);
                return View(carrinho);
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

        [Authorize]
        public async Task<IActionResult> Index(Dictionary<string, int> quantidades, string action)
        {
            try
            {
                var usuario = appUserParser.Parse(HttpContext.User);
                var carrinho = carrinhoService.DefinirQuantidades(usuario, quantidades);
                //if (action == "[ Checkout ]")
                //{
                //    RedirectToAction("Create", "Order");
                //}
            }
            catch (BrokenCircuitException ex)
            {
                HandleBrokenCircuitException();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<UpdateQuantidadeOutput> UpdateQuantidade([FromBody]ItemCarrinho itemCarrinho)
        {
            return await carrinhoService.UpdateItem(GetUserId(), itemCarrinho);
        }

    }
}
