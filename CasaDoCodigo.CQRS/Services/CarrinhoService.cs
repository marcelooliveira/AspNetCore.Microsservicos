using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            public static string GetCarrinho => "api/carrinho"; //carrinho
            public static string PostItem => "api/carrinho"; //carrinho
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

        public async Task<CarrinhoViewModel> GetCarrinho(string userId)
        {
            return await GetAsync<CarrinhoViewModel>(CarrinhoUris.GetCarrinho, userId);
        }

        public async Task<CarrinhoViewModel> UpdateItem(string clienteId, ItemCarrinho input)
        {
            var uri = $"{CarrinhoUris.PostItem}/{clienteId}";
            return await PostAsync<CarrinhoViewModel>(uri, input);
        }

        protected override string Scope => "CasaDoCodigo.Carrinho";
    }
}
