using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MVC.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientAuthorizationDelegatingHandler(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccesor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccesor = httpContextAccesor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccesor.HttpContext
                .Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }

            var token = await GetToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        async Task<string> GetToken()
        {
            const string ACCESS_TOKEN = "access_token";

            return await _httpContextAccesor.HttpContext
                .GetTokenAsync(ACCESS_TOKEN);
        }
    }
}
