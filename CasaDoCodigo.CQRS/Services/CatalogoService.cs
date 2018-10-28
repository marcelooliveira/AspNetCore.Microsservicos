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
    public class CatalogoService : BaseHttpService, ICatalogoService
    {
        class ApiUris
        {
            public static string GetProdutos => "api/produto";
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
            var uri = _baseUri + ApiUris.GetProdutos;
            var json = await _httpClient.GetStringAsync(uri);
            IList<Produto> result = JsonConvert.DeserializeObject<IList<Models.Produto>>(json);
            return result;
        }

        public async Task<Models.Produto> GetProduto(string codigo)
        {
            Produto result = await GetAsync<Models.Produto>(ApiUris.GetProdutos, codigo);
            return result;
        }

        protected override string Scope => "CasaDoCodigo.Catalogo";
    }
}
