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
            , HttpClient httpClient
            , ISessionHelper sessionHelper
            , ILogger<ApiService> logger)
            : base(configuration, httpClient, sessionHelper)
        {
            _logger = logger;
            _baseUri = _configuration["ApiUrl"];
        }

        public async Task<CarrinhoCliente> Carrinho(string codigo, int pedidoId)
        {
            return await GetAsync<CarrinhoCliente>(ApiUris.GetCarrinho, pedidoId, codigo);
        }

        public async Task<PedidoViewModel> GetPedido(int pedidoId)
        {
            return await GetAsync<PedidoViewModel>(ApiUris.GetPedido, pedidoId);
        }

        public async Task<PedidoViewModel> UpdateCadastro(Models.CadastroViewModel viewModel)
        {
            return await PostAsync<PedidoViewModel>(ApiUris.UpdateCadastro, viewModel);
        }

        protected override string Scope => "CasaDoCodigo.API";
    }
}
