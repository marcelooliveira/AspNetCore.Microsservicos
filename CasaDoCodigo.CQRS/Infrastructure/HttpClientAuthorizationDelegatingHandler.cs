using CasaDoCodigo.Client.Generated;
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
        private CasaDoCodigo.Client.Generated.Client apiCliente;

        public HttpClientAuthorizationDelegatingHandler(IConfiguration configuration,
            IHttpContextAccessor httpContextAccesor)
        {
            _configuration = configuration;
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
                using (HttpClient httpClient = new HttpClient())
                {
                    apiCliente = new Client.Generated.Client(_configuration["ApiUrl"], httpClient);
                    token = await apiCliente.ApiLoginPostAsync(
                    new UsuarioInput
                    {
                        UsuarioId = Environment.GetEnvironmentVariable("CASADOCODIGO_USERID", EnvironmentVariableTarget.User),
                        Password = Environment.GetEnvironmentVariable("CASADOCODIGO_PASSWORD", EnvironmentVariableTarget.User),
                    });
                }
                _httpContextAccesor.HttpContext.Session.SetString("accessToken", token);
            }
            return token;
        }
    }
}
