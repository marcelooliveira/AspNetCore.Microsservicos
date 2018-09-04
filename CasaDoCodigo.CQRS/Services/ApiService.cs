using CasaDoCodigo.Client.API.Generated;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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

namespace CasaDoCodigo.Services
{
    public class ApiService : IApiService
    {
        class ApiUris
        {
            public static string GetProdutos => "api/produto";
            public static string GetCarrinho => "api/carrinho";
            public static string GetPedido => "api/pedido";
            public static string UpdateQuantidade => "api/carrinho";
            public static string UpdateCadastro => "api/cadastro";
        }

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _baseUri;

        public ApiService(
            IConfiguration configuration
            , HttpClient httpClient
            , ILogger<ApiService> logger
            , IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _baseUri = _configuration["ApiUrl"];
        }

        public async Task<IEnumerable<Models.Produto>> GetProdutos()
        {
            var uri = _baseUri + ApiUris.GetProdutos;
            var result = await _httpClient.GetStringAsync(uri);
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

        private async Task<T> GetAsync<T>(string uri, params object[] param)
        {
            string requestUri = string.Format(_baseUri + uri, param);

            foreach (var par in param)
            {
                requestUri += string.Format($"/{par}");
            }

            var json = await _httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task<T> PostAsync<T>(string uri, object content)
        {
            var jsonIn = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonIn, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await _httpClient.PostAsync(_baseUri + uri, stringContent);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = new { httpResponse.StatusCode, httpResponse.ReasonPhrase };
                var errorJson = JsonConvert.SerializeObject(error);
                throw new HttpRequestException(errorJson);
            }
            var jsonOut = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonOut);
        }
    }
}
