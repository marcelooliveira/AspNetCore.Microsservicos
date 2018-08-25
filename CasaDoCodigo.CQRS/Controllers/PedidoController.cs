using CasaDoCodigo.Client.Generated;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private CasaDoCodigo.Client.Generated.Client apiCliente;
        private string accessToken;

        public IConfiguration Configuration { get; }

        public PedidoController(ILogger<PedidoController> logger,
            IHttpContextAccessor contextAccessor,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.contextAccessor = contextAccessor;
            this.httpClient = httpClient;
            this.Configuration = configuration;
        }

        public async Task<IActionResult> Carrossel()
        {
            try
            {
                //return View(await GetAsync<IList<Produto>>(ApiUris.GetProdutos));

                using (var httpClient = new HttpClient())
                {
                    apiCliente = new CasaDoCodigo.Client.Generated.Client(Configuration["ApiUrl"], httpClient);
                    if (string.IsNullOrWhiteSpace(accessToken))
                    {
                        accessToken = await ObterToken();
                    }

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
                    return View(await ObterProdutos());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "Carrossel");
                throw;
            }
        }

        private async Task<string> ObterToken()
        {
            var token = contextAccessor.HttpContext.Session.GetString("accessToken");

            if (token == null)
            {
                token = await apiCliente.ApiLoginPostAsync(
                new UsuarioInput
                {
                    UsuarioId = Environment.GetEnvironmentVariable("CASADOCODIGO_USERID", EnvironmentVariableTarget.User),
                    Password = Environment.GetEnvironmentVariable("CASADOCODIGO_PASSWORD", EnvironmentVariableTarget.User),
                });
                contextAccessor.HttpContext.Session.SetString("accessToken", token);
            }
            return token;
        }

        private async Task<IList<Models.Produto>> ObterProdutos()
        {
            Console.WriteLine("Obtendo produtos...");
            var resultado = await apiCliente.ApiProdutoGetAsync();
            return resultado.Select(r =>
                new Models.Produto(r.Id.Value, r.Codigo, r.Nome, (decimal)(r.Preco.Value)))
                .ToList();
        }

        public async Task<IActionResult> Carrinho(string codigo)
        {
            try
            {
                int pedidoId = GetPedidoId() ?? 0;
                CarrinhoViewModel carrinho = await GetAsync<CarrinhoViewModel>(ApiUris.GetCarrinho, pedidoId, codigo);
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
            try
            {
                if (ModelState.IsValid)
                {
                    var viewModel = new CasaDoCodigo.Models.CadastroViewModel(cadastro);
                    viewModel.PedidoId = GetPedidoId().Value;
                    var pedidoViewModel = await PostAsync<PedidoViewModel>(ApiUris.UpdateCadastro, viewModel);
                    return base.View(pedidoViewModel);
                }
                return RedirectToAction("Cadastro");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "Resumo");
                throw;
            }
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
