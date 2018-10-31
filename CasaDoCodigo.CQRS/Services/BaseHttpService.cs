using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace CasaDoCodigo.Services
{
    delegate Task<HttpResponseMessage> HttpVerbMethod(Uri requestUri, HttpContent content);
    
    public abstract class BaseHttpService : IService
    {
        protected readonly IConfiguration _configuration;
        protected readonly HttpClient _httpClient;
        protected readonly ISessionHelper _sessionHelper;
        protected string _baseUri;

        public BaseHttpService(IConfiguration configuration, HttpClient httpClient, ISessionHelper sessionHelper)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _sessionHelper = sessionHelper;
        }

        protected async Task<T> GetAsync<T>(string uri, params object[] param)
        {
            string requestUri = 
                string.Format(new Uri(new Uri(_baseUri), uri).ToString(), param);

            foreach (var par in param)
            {
                requestUri += string.Format($"/{par}");
            }

            var json = await _httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected async Task<T> PostAsync<T>(string uri, object content)
        {
            HttpVerbMethod httpVerbMethod = new HttpVerbMethod(_httpClient.PostAsync);
            return await PutOrPostAsync<T>(uri, content, httpVerbMethod);
        }

        protected async Task<T> PutAsync<T>(string uri, object content)
        {
            HttpVerbMethod httpVerbMethod = new HttpVerbMethod(_httpClient.PutAsync);
            return await PutOrPostAsync<T>(uri, content, httpVerbMethod);
        }

        private async Task<T> PutOrPostAsync<T>(string uri, object content, HttpVerbMethod httpVerbMethod)
        {
            var jsonIn = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonIn, Encoding.UTF8, "application/json");

            var accessToken = await _sessionHelper.GetAccessToken(Scope);
            _httpClient.SetBearerToken(accessToken);

            HttpResponseMessage httpResponse = await httpVerbMethod(new Uri(new Uri(_baseUri), uri), stringContent);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = new { httpResponse.StatusCode, httpResponse.ReasonPhrase };
                var errorJson = JsonConvert.SerializeObject(error);
                throw new HttpRequestException(errorJson);
            }
            var jsonOut = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonOut);
        }

        public abstract string Scope { get; }
    }
}
