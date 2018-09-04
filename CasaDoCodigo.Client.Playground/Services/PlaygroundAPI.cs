using CasaDoCodigo.Client.API.Generated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CasaDoCodigo.Client.Playground.Services
{
    class PlaygroundAPI : BasePlayground
    {
        private Client.API.Generated.Client cliente;

        public PlaygroundAPI(ApiConfiguration configuration) :
            base(configuration)
        {

        }

        protected override async Task ExecutarPlayground()
        {
            cliente = new Client.API.Generated.Client(ApiConfiguration.BaseUrlAPI, httpClient);
            await ListarProdutos();
        }

        private async Task ListarProdutos()
        {
            Console.Clear();
            ImprimirListagem(await ObterProdutos());
            Console.WriteLine("Tecle algo para sair");
            Console.ReadKey();
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
