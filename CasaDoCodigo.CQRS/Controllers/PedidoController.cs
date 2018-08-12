using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    class ApiUris
    {
        public static string GetProdutos => "api/produto";
        public static string GetCarrinho => "api/carrinho";
    }

    public class PedidoController : Controller
    {
        private readonly ILogger logger;
        private readonly IProdutoRepository produtoRepository;
        private readonly IPedidoRepository pedidoRepository;
        private readonly IItemPedidoRepository itemPedidoRepository;
        private readonly HttpClient httpClient;

        public PedidoController(ILogger<PedidoController> logger,
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            IItemPedidoRepository itemPedidoRepository,
            HttpClient httpClient)
        {
            this.logger = logger;
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
            this.itemPedidoRepository = itemPedidoRepository;
            this.httpClient = httpClient;
        }

        public async Task<IActionResult> Carrossel()
        {
            return View(await GetAsync<IList<Produto>>(ApiUris.GetProdutos));
        }

        private async Task<T> GetAsync<T>(string uri, params object[] param)
        {
            string requestUri = string.Format(uri, param);

            foreach (var par in param)
            {
                requestUri += string.Format($"/{par}");
            }

            var json = await httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<IActionResult> Carrinho(string codigo)
        {
            try
            {
                return View(await GetAsync<CarrinhoViewModel>(ApiUris.GetCarrinho, codigo));
            }
            catch (Exception exc)
            {
                logger.LogError(exc, exc.Message, "Carrinho");
                throw;
            }

            //if (!string.IsNullOrEmpty(codigo))
            //{
            //    await pedidoRepository.AddItem(codigo);
            //}

            //Pedido pedido = await pedidoRepository.GetPedido();
            //List<ItemPedido> itens = pedido.Itens;
            //return base.View(new CarrinhoViewModel(itens));
        }

        public async Task<IActionResult> Cadastro()
        {
            var pedido = await pedidoRepository.GetPedido();

            if (pedido == null)
            {
                return RedirectToAction("Carrossel");
            }

            return View(pedido.Cadastro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resumo(Cadastro cadastro)
        {
            if (ModelState.IsValid)
            {
                return View(await pedidoRepository.UpdateCadastro(cadastro));
            }
            return RedirectToAction("Cadastro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<UpdateQuantidadeResponse> UpdateQuantidade([FromBody]ItemPedido itemPedido)
        {
            return await pedidoRepository.UpdateQuantidade(itemPedido);
        }
    }
}
