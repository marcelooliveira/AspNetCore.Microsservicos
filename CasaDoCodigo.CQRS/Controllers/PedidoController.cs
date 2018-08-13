using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    class ApiUris
    {
        public static string GetProdutos => "api/produto";
        public static string GetCarrinho => "api/carrinho";
        public static string GetPedido => "api/pedido";
        public static string UpdateQuantidade => "api/carrinho";
    }

    public class PedidoController : Controller
    {
        private readonly ILogger logger;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IProdutoRepository produtoRepository;
        private readonly IPedidoRepository pedidoRepository;
        private readonly IItemPedidoRepository itemPedidoRepository;
        private readonly HttpClient httpClient;

        public PedidoController(ILogger<PedidoController> logger,
            IHttpContextAccessor contextAccessor,
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            IItemPedidoRepository itemPedidoRepository,
            HttpClient httpClient)
        {
            this.logger = logger;
            this.contextAccessor = contextAccessor;
            this.produtoRepository = produtoRepository;
            this.pedidoRepository = pedidoRepository;
            this.itemPedidoRepository = itemPedidoRepository;
            this.httpClient = httpClient;
        }

        public async Task<IActionResult> Carrossel()
        {
            try
            {
                return View(await GetAsync<IList<Produto>>(ApiUris.GetProdutos));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "Carrossel");
                throw;
            }
        }

        public async Task<IActionResult> Carrinho(string codigo)
        {
            try
            {
                CarrinhoViewModel carrinho = await GetAsync<CarrinhoViewModel>(ApiUris.GetCarrinho, codigo);
                SetPedidoId(carrinho.PedidoId);
                return base.View(carrinho);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "Carrinho");
                throw;
            }
        }

        public async Task<IActionResult> Cadastro()
        {
            try
            {
                int pedidoId = GetPedidoId() ?? throw new ArgumentNullException("pedidoId");
                PedidoViewModel pedido = await GetAsync<PedidoViewModel>(ApiUris.GetPedido, pedidoId);

                if (pedido == null)
                {
                    return RedirectToAction("Carrossel");
                }

                return View(pedido.Cadastro);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "Cadastro");
                throw;
            }
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
        public async Task<UpdateQuantidadeOutput> UpdateQuantidade([FromBody]ItemPedido itemPedido)
        {
            try
            {
                return await PostAsync<UpdateQuantidadeOutput>(
                    ApiUris.UpdateQuantidade,
                    new { ItemPedidoId = itemPedido.Id, itemPedido.Quantidade });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "UpdateQuantidade");
                throw;
            }
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

        private async Task<T> PostAsync<T>(string uri, object content)
        {
            var jsonIn = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonIn, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await httpClient.PostAsync(uri, stringContent);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = new { httpResponse.StatusCode, httpResponse.ReasonPhrase };
                var errorJson = JsonConvert.SerializeObject(error);
                throw new HttpRequestException(errorJson);
            }
            var jsonOut = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonOut);
        }

        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }
    }
}
