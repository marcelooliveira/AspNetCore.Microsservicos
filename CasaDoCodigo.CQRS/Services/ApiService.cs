using CasaDoCodigo.Client.API.Generated;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public class ApiService : BaseHttpService, IApiService
    {
        class ApiUris
        {
            public static string GetProdutos => "api/produto";
            public static string GetCarrinho => "api/carrinho";
            public static string GetPedido => "api/pedido";
            public static string UpdateQuantidade => "api/carrinho";
            public static string UpdateCadastro => "api/cadastro";
        }

        private readonly ILogger<ApiService> _logger;

        public ApiService(
            IConfiguration configuration
            , IHttpClientFactory httpClientFactory
            , HttpClient httpClient
            , IHttpContextAccessor contextAccessor
            , ILogger<ApiService> logger) 
            : base(configuration, httpClientFactory, httpClient, contextAccessor)
        {
            _logger = logger;
            _baseUri = _configuration["ApiUrl"];
        }

        public async Task<IEnumerable<Models.Produto>> GetProdutos()
        {
            var uri = _baseUri + ApiUris.GetProdutos;
            var client = _httpClientFactory.CreateClient();
            var result = await client.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<IEnumerable<Models.Produto>>(result);
        }

        public async Task<CarrinhoViewModel> Carrinho(string codigo, int pedidoId)
        {
            return await GetAsync<CarrinhoViewModel>(ApiUris.GetCarrinho, pedidoId, codigo);
        }

        public async Task<PedidoViewModel> GetPedido(int pedidoId)
        {
            return await GetAsync<PedidoViewModel>(ApiUris.GetPedido, pedidoId);
        }

        public async Task<PedidoViewModel> UpdateCadastro(Models.CadastroViewModel viewModel)
        {
            return await PostAsync<PedidoViewModel>(ApiUris.UpdateCadastro, viewModel);
        }

        public async Task<UpdateQuantidadeOutput> UpdateQuantidade(int itemPedidoId, int quantidade)
        {
            return await PostAsync<UpdateQuantidadeOutput>(
                ApiUris.UpdateQuantidade, new { itemPedidoId, quantidade });
        }

    }
}
