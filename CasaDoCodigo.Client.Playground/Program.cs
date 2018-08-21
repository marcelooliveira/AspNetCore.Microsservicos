using CasaDoCodigo.Client.Generated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CasaDoCodigo.Client.Playground
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json");

            var config = builder.Build();

            var configuration = config.GetSection("ApiConfiguration").Get<ApiConfiguration>();

            Console.WriteLine("Iniciando...");
            var relatorio = new Relatorio(configuration);
            await relatorio.Executar();
            Console.WriteLine("Tecle algo para sair...");
            Console.ReadKey();
        }

    }

    class Relatorio
    {
        private const int DELAY_SEGUNDOS = 1;
        private Generated.Client cliente;
        private string accessToken;
        private HttpClient httpClient;
        private ApiConfiguration ApiConfiguration { get; set; }

        public Relatorio(ApiConfiguration configuration)
        {
            ApiConfiguration = configuration;
        }

        public async Task Executar()
        {
            await ListarProdutos();
        }

        private async Task ListarProdutos()
        {
            ImprimirLogo();
            using (var httpClient = new HttpClient())
            {
                cliente = new Generated.Client(ApiConfiguration.BaseUrl, httpClient);
                accessToken = await ObterToken();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

                char key;
                do
                {
                    Console.Clear();
                    ImprimirListagem(await ObterProdutos());
                    //await Task.Delay(DELAY_SEGUNDOS);
                    Console.WriteLine("Tecle S para sair ou outra teclar para ");
                    key = Console.ReadKey().KeyChar;
                }
                while (key != 'S');
            }
        }

        private void ImprimirLogo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
@"
 .d8888b.                                 888            .d8888b.              888d8b                
d88P  Y88b                                888           d88P  Y88b             888Y8P                
888    888                                888           888    888             888                   
888        8888b. .d8888b  8888b.     .d88888 .d88b.    888        .d88b.  .d88888888 .d88b.  .d88b. 
888           '88b88K         '88b   d88' 888d88''88b   888       d88''88bd88' 888888d88P'88bd88''88b
888    888.d888888'Y8888b..d888888   888  888888  888   888    888888  888888  888888888  888888  888
Y88b  d88P888  888     X88888  888   Y88b 888Y88..88P   Y88b  d88PY88..88PY88b 888888Y88b 888Y88..88P
 'Y8888P' 'Y888888 88888P''Y888888    'Y88888 'Y88P'     'Y8888P'  'Y88P'  'Y88888888 'Y88888 'Y88P' 
                                                                                      'Y88P' 
");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private async Task<string> ObterToken()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Obtendo token...");
                    return await cliente.ApiLoginPostAsync(
                        new User
                        {
                            //Id = "eed37679-a43c-4d59-8a27-50fc710834ad",
                            //PasswordHash = "AQAAAAEAACcQAAAAEHVpHiMNMZFTMQ0YAGEYmYz24hdervKcEaQBxIl5XcStRg7azq66UjXNyNVaow3dWA=="
                            Id = "5ef851c5-c3e1-46c0-8311-c0521e188bf7",
                            PasswordHash = "AQAAAAEAACcQAAAAEGBAFYz4d71CwppE+I4H1XBpLrV9+8TOWh1HpmojIgdvMdEAnpa1JFoPHtUwoG1Odg=="
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao acessar {ApiConfiguration.BaseUrl}:\r\n" +
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
