using CasaDoCodigo.Client.Generated;
using CasaDoCodigo.Models;
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
    }
}
