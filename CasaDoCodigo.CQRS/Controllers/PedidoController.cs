using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using System;
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
        public static string UpdateCadastro => "api/cadastro";
    }

    public class PedidoController : Controller
    {
        private readonly ILogger logger;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly HttpClient httpClient;
        private readonly IApiService apiService;
        private readonly ICarrinhoService carrinhoService;

        public IConfiguration Configuration { get; }

        public PedidoController(ILogger<PedidoController> logger,
            IHttpContextAccessor contextAccessor,
            HttpClient httpClient,
            IConfiguration configuration,
            ICarrinhoService carrinhoService,
            IApiService apiService)
        {
            this.logger = logger;
            this.contextAccessor = contextAccessor;
            this.httpClient = httpClient;
            this.Configuration = configuration;
            this.carrinhoService = carrinhoService;
            this.apiService = apiService;
        }

        public async Task<IActionResult> Carrossel()
        {
            try
            {
                return View(await apiService.GetProdutos());
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

        public async Task<IActionResult> Carrinho(string codigoProduto)
        {
            try
            {

                int pedidoId = GetPedidoId() ?? 0;
                var idUsuario = "eed37679-a43c-4d59-8a27-50fc710834ad";
                ItemCarrinho itemCarrinho = new ItemCarrinho("", codigoProduto, $"produto código {codigoProduto}", 1.23m, 1, "");
                var carrinho = await carrinhoService.UpdateItem(idUsuario, itemCarrinho);
                //SetPedidoId(carrinho.PedidoId);
                return View(carrinho);
            }
            catch (BrokenCircuitException)
            {
                HandleBrokenCircuitException();
            }
            return View();
        }

        public async Task<IActionResult> Cadastro()
        {
            try
            {
                int pedidoId = GetPedidoId() ?? throw new ArgumentNullException("pedidoId");
                PedidoViewModel pedido = await apiService.GetPedido(pedidoId);

                if (pedido == null)
                {
                    return RedirectToAction("Carrossel");
                }

                return View(pedido.Cadastro);
            }
            catch (BrokenCircuitException)
            {
                HandleBrokenCircuitException();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resumo(Cadastro cadastro)
        {
            if (ModelState.IsValid)
            {
                var viewModel = new CasaDoCodigo.Models.CadastroViewModel(cadastro);
                viewModel.PedidoId = GetPedidoId().Value;
                var pedidoViewModel = await apiService.UpdateCadastro(viewModel);
                return base.View(pedidoViewModel);
            }
            return RedirectToAction("Cadastro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<UpdateQuantidadeOutput> UpdateQuantidade([FromBody]ItemPedido itemPedido)
        {
            return await apiService.UpdateQuantidade(itemPedido.Id, itemPedido.Quantidade);
        }

        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }

        private void HandleBrokenCircuitException()
        {
            ViewBag.MsgCarrosselIndisponivel = "O serviço de carrossel não está ativo, por favor tente novamente mais tarde.";
        }
    }
}
