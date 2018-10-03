using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public class BaseHttpService
    {
        protected readonly IConfiguration _configuration;
        protected readonly HttpClient _httpClient;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IHttpContextAccessor _contextAccessor;
        protected string _baseUri;

        public BaseHttpService(IConfiguration configuration, IHttpClientFactory httpClientFactory, HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        protected async Task<T> GetAsync<T>(string uri, params object[] param)
        {
            string requestUri = 
                string.Format(new Uri(new Uri(_baseUri), uri).ToString(), param);

            foreach (var par in param)
            {
                requestUri += string.Format($"/{par}");
            }

            //var client = _httpClientFactory.CreateClient();
            var json = await _httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected async Task<T> PostAsync<T>(string uri, object content)
        {
            var jsonIn = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonIn, Encoding.UTF8, "application/json");

            //var client = _httpClientFactory.CreateClient();
            HttpResponseMessage httpResponse = await _httpClient.PostAsync(new Uri(new Uri(_baseUri), uri), stringContent);
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
