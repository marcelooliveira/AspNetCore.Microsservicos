using MVC.Models;
using MVC.Models.ViewModels;
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

namespace MVC.Services
{
    public class CatalogoService : BaseHttpService, ICatalogoService
    {
        class ApiUris
        {
            public static string GetProduto => "api/produto";
            public static string BuscaProdutos => "api/busca";
        }

        private readonly ILogger<CatalogoService> _logger;

        public CatalogoService(
            IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper
            , ILogger<CatalogoService> logger)
            : base(configuration, httpClient, sessionHelper)
        {
            _logger = logger;
            _baseUri = _configuration["CatalogoUrl"];
        }

        public async Task<IList<Models.Produto>> GetProdutos()
        {
            var uri = _baseUri + ApiUris.GetProduto;
            var json = await _httpClient.GetStringAsync(uri);
            IList<Produto> result = JsonConvert.DeserializeObject<IList<Models.Produto>>(json);
            return result;
        }

        public async Task<IList<Produto>> BuscaProdutos(string pesquisa)
        {
            return await GetAsync<List<Produto>>(ApiUris.BuscaProdutos, pesquisa);
        }

        public async Task<Models.Produto> GetProduto(string codigo)
        {
            return await GetAsync<Produto>(ApiUris.GetProduto, codigo);
        }

        public override string Scope => "Catalogo.API";
    }
}
