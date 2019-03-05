using MVC.Infrastructure;
using MVC.Models;
using MVC.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MVC.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVC.Services
{
    public class CarrinhoService : BaseHttpService, ICarrinhoService
    {
        class CarrinhoUris
        {
            public static string GetCarrinho => "api/carrinho";
            public static string AddItem => "api/carrinho/additem";
            public static string UpdateItem => "api/carrinho/updateitem";
            public static string Finalizar => "api/carrinho/checkout";
            //public static string GetCarrinho => "carrinho"; //ApiGateway
            //public static string PostItem => "carrinho"; //ApiGateway
        }

        private readonly HttpClient _apiClient;
        private readonly string _carrinhoUrl;
        private readonly ILogger<CarrinhoService> _logger;

        public CarrinhoService(
            IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper
            , ILogger<CarrinhoService> logger)
            : base(configuration, httpClient, sessionHelper)
        {
            _apiClient = httpClient;
            _logger = logger;
            _baseUri = _configuration["CarrinhoUrl"];
        }

        public async Task<CarrinhoCliente> GetCarrinho(string userId)
        {
            return await GetAsync<CarrinhoCliente>(CarrinhoUris.GetCarrinho, userId);
        }

        public async Task<CarrinhoCliente> AddItem(string clienteId, ItemCarrinho input)
        {
            var uri = $"{CarrinhoUris.AddItem}/{clienteId}";
            return await PostAsync<CarrinhoCliente>(uri, input);
        }

        public async Task<UpdateQuantidadeOutput> UpdateItem(string clienteId, UpdateQuantidadeInput input)
        {
            var uri = $"{CarrinhoUris.UpdateItem}/{clienteId}";
            return await PutAsync<UpdateQuantidadeOutput>(uri, input);
        }

        public async Task<CarrinhoCliente> DefinirQuantidades(ApplicationUser applicationUser, Dictionary<string, int> quantidades)
        {
            var uri = UrlAPIs.Carrinho.UpdateItemCarrinho(_carrinhoUrl);

            var atualizarCarrinho = new
            {
                ClienteId = applicationUser.Id,
                Atualizacao = quantidades.Select(kvp => new
                {
                    ItemCarrinhoId = kvp.Key,
                    NovaQuantidade = kvp.Value
                }).ToArray()
            };

            var conteudoCarrinho = new StringContent(JsonConvert.SerializeObject(atualizarCarrinho), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PutAsync(uri, conteudoCarrinho);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CarrinhoCliente>(jsonResponse);
        }

        public Task AtualizarCarrinho(CarrinhoCliente carrinhoCliente)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Checkout(string clienteId, CadastroViewModel viewModel)
        {
            var uri = $"{CarrinhoUris.Finalizar}/{clienteId}";
            return await PostAsync<bool>(uri, viewModel);
        }

        public override string Scope => "Carrinho.API";
    }
}
