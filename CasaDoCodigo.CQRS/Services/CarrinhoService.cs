using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.Services
{
    public class CarrinhoService : BaseHttpService, ICarrinhoService
    {
        class CarrinhoUris
        {
            public static string GetCarrinho => "api/carrinho";
            public static string AddItem => "api/carrinho/additem";
            public static string UpdateItem => "api/carrinho/updateitem";
            //public static string GetCarrinho => "carrinho"; //ApiGateway
            //public static string PostItem => "carrinho"; //ApiGateway
        }

        private readonly ILogger<CarrinhoService> _logger;

        public CarrinhoService(
            IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper
            , ILogger<CarrinhoService> logger)
            : base(configuration, httpClient, sessionHelper)
        {
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

        public async Task<UpdateQuantidadeOutput> UpdateItem(string clienteId, ItemCarrinho input)
        {
            var uri = $"{CarrinhoUris.UpdateItem}/{clienteId}";
            return await PostAsync<UpdateQuantidadeOutput>(uri, input);
        }

        protected override string Scope => "CasaDoCodigo.Carrinho";
    }
}
