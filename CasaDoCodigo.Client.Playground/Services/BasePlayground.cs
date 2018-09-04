using CasaDoCodigo.Client.API.Generated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CasaDoCodigo.Client.Playground.Services
{
    abstract class BasePlayground
    {
        protected HttpClient httpClient;
        protected string accessToken;
        protected const int DELAY_SEGUNDOS = 1;
        protected Client.API.Generated.Client authClient;
        protected ApiConfiguration ApiConfiguration { get; set; }

        public BasePlayground(ApiConfiguration configuration)
        {
            ApiConfiguration = configuration;
            httpClient = new HttpClient();
        }

        public async Task Executar()
        {
            await SetupHttpClient();
            await ExecutarPlayground();
        }

        protected abstract Task ExecutarPlayground();

        protected async Task<string> ObterToken()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Obtendo token...");

                    return await authClient.ApiLoginPostAsync(
                        new UsuarioInput
                        {
                            UsuarioId = Environment.GetEnvironmentVariable("CASADOCODIGO_USERID", EnvironmentVariableTarget.User),
                            Password = Environment.GetEnvironmentVariable("CASADOCODIGO_PASSWORD", EnvironmentVariableTarget.User),
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao acessar {ApiConfiguration.BaseUrlAPI}:\r\n" +
                        $"{ex.Message}\r\n" +
                        $"tentando novamente em {DELAY_SEGUNDOS}s");
                    await Task.Delay(TimeSpan.FromSeconds(DELAY_SEGUNDOS));
                }
            }
        }

        protected async Task SetupHttpClient()
        {
            authClient = new Client.API.Generated.Client(ApiConfiguration.BaseUrlAPI, httpClient);
            accessToken = await ObterToken();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        }

    }
}
