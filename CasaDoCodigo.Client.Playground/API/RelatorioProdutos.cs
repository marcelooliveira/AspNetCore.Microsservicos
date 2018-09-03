using CasaDoCodigo.Client.API.Generated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CasaDoCodigo.Client.Playground.API
{
    class RelatorioProdutos
    {
        private const int DELAY_SEGUNDOS = 1;
        private Client.API.Generated.Client cliente;
        private string accessToken;
        private ApiConfiguration ApiConfiguration { get; set; }

        public RelatorioProdutos(ApiConfiguration configuration)
        {
            ApiConfiguration = configuration;
        }

        public async Task Executar()
        {
            await ListarProdutos();
        }

        private async Task ListarProdutos()
        {
            using (var httpClient = new HttpClient())
            {
                cliente = new Client.API.Generated.Client(ApiConfiguration.BaseUrlAPI, httpClient);
                accessToken = await ObterToken();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

                Console.Clear();
                ImprimirListagem(await ObterProdutos());
                Console.WriteLine("Tecle algo para sair");
                Console.ReadKey();
            }
        }


        private async Task<string> ObterToken()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Obtendo token...");

                    return await cliente.ApiLoginPostAsync(
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

        private async Task<IList<Produto>> ObterProdutos()
        {
            IList<Produto> produtos = null;
            var sucesso = false;
            while (!sucesso)
            {
                try
                {
                    Console.WriteLine("Obtendo produtos...");
                    produtos = await cliente.ApiProdutoGetAsync();
                    sucesso = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao obter produtos:\r\n" +
                        $"{ex.Message}\r\n" +
                        $"tentando novamente em 5s");
                    await Task.Delay(TimeSpan.FromSeconds(DELAY_SEGUNDOS));
                }
            }

            return produtos;
        }

        private void ImprimirListagem(IList<Produto> produtos)
        {
            Console.WriteLine(new string('=', 50));
            foreach (var produto in produtos)
            {
                Console.WriteLine(
                    $"Id: {produto.Id}\r\n" +
                    $"Codigo:{produto.Codigo}\r\n" +
                    $"Nome:{produto.Nome}\r\n" +
                    $"Preco:{produto.Preco:C}");
                Console.WriteLine(new string('=', 50));
            }
        }
    }
}
