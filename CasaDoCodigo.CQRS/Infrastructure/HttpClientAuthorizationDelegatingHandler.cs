using CasaDoCodigo.Client.API.Generated;
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

namespace CasaDoCodigo.Infrastructure
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IHttpClientFactory _httpClientFactory;
        private CasaDoCodigo.Client.API.Generated.Client apiCliente;

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
            var token = await GetToken();

            if (token != null)
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        async Task<string> GetToken()
        {
            var token = _httpContextAccesor.HttpContext.Session.GetString("accessToken");

            if (token == null)
            {
                var cliente = _httpClientFactory.CreateClient();
                apiCliente = new Client.API.Generated.Client(_configuration["ApiUrl"], cliente);
                token = await apiCliente.ApiLoginPostAsync(
                new UsuarioInput
                {
                    UsuarioId = Environment.GetEnvironmentVariable("CASADOCODIGO_USERID", EnvironmentVariableTarget.User),
                    Password = Environment.GetEnvironmentVariable("CASADOCODIGO_PASSWORD", EnvironmentVariableTarget.User),
                });
                _httpContextAccesor.HttpContext.Session.SetString("accessToken", token);
            }
            return token;
        }
    }
}
