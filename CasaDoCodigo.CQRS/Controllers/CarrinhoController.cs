using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CarrinhoController : BaseController
    {
        private readonly ICatalogoService catalogoService;
        private readonly ICarrinhoService carrinhoService;

        public CarrinhoController(ICatalogoService catalogoService, ICarrinhoService carrinhoService, IHttpContextAccessor contextAccessor)
            : base(contextAccessor)
        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<UpdateQuantidadeOutput> UpdateQuantidade([FromBody]ItemCarrinho itemCarrinho)
        {
            return await carrinhoService.UpdateItem(GetUserId(), itemCarrinho);
        }
    }
}
