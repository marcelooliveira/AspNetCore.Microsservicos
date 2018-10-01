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
            public static string GetCarrinho => "api/carrinho";
            public static string PostCarrinho => "api/carrinho/postitem";
        }

        private readonly ILogger<CarrinhoService> _logger;

        public CarrinhoService(
            IConfiguration configuration
            , HttpClient httpClient
            , IHttpContextAccessor contextAccessor
            , ILogger<CarrinhoService> logger)
            : base(configuration, httpClient, contextAccessor)
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
            var uri = new Uri(new Uri(CarrinhoUris.PostCarrinho), clienteId);
            return await PostAsync<CarrinhoViewModel>(uri.ToString(), input);
        }
    }
}
