using CasaDoCodigo.Client.Playground.API;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CasaDoCodigo.Client.Playground
{
    public delegate Task ItemMenu(ApiConfiguration configuration);
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

            ItemMenu[] itensMenu = new ItemMenu[]
            {
                ImprimirRelatorioProdutos
            };

            int opcao;
            do
            {
                ImprimirMenu(itensMenu);
                var linha = Console.ReadLine();
                int.TryParse(linha, out opcao);
                if (opcao > 0)
                    await itensMenu[opcao - 1](configuration);
            } while (opcao > 0);
        }

        private static void ImprimirMenu(ItemMenu[] itensMenu)
        {
            Console.Clear();
            ImprimirLogo();
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine();
            for (int i = 0; i < itensMenu.Length; i++)
            {
                ItemMenu itemMenu = itensMenu[i];
                Console.WriteLine($"{i + 1} - {itemMenu.Method.Name}");
            }
            Console.WriteLine($"0 - Sair");
        }

        private static void ImprimirLogo()
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

        private static async Task ImprimirRelatorioProdutos(ApiConfiguration configuration)
        {
            var relatorio = new RelatorioProdutos(configuration);
            await relatorio.Executar();
        }
    }
}
