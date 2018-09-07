using CasaDoCodigo.Client.Carrinho.Generated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CasaDoCodigo.Client.Playground.Services
{
    class PlaygroundCarrinho : BasePlayground
    {
        protected Client.Carrinho.Generated.Client carrinhoClient;

        public PlaygroundCarrinho(ApiConfiguration configuration) :
            base(configuration)
        {

        }

        protected async override Task ExecutarPlayground()
        {
            carrinhoClient = new Carrinho.Generated.Client(ApiConfiguration.BaseUrlCarrinho, httpClient);

            System.Console.WriteLine("ApiCarrinhoPostAsync...");
            CarrinhoCliente carrinho = new CarrinhoCliente
            {
                ClienteId = usuarioInput.UsuarioId,
                Items = new ObservableCollection<ItemCarrinho>(new List<ItemCarrinho>
                {
                    new ItemCarrinho { Id = "1", PrecoUnitario = 12.34, ProdutoId = "1", ProdutoNome = "Java para baixinhos", Quantidade = 2, UrlImagem = "http:\\img1.images.com}" },
                    new ItemCarrinho { Id = "2", PrecoUnitario = 23.45, ProdutoId = "2", ProdutoNome = "C# for dummies", Quantidade = 3, UrlImagem = "http:\\img2.images.com}" }
                })
            };

            await carrinhoClient.ApiCarrinhoPostAsync(carrinho);

            System.Console.WriteLine("ApiCarrinhoByIdGetAsync...");
            var carrinhoCliente = await carrinhoClient.ApiCarrinhoByIdGetAsync(usuarioInput.UsuarioId);
            PrintCarrinho(carrinhoCliente);

            System.Console.WriteLine("ApiCarrinhoPostAsync...");
            carrinho.Items[0].Quantidade = 777;
            carrinho.Items[1].Quantidade = 888;
            await carrinhoClient.ApiCarrinhoPostAsync(carrinho);
            carrinhoCliente = await carrinhoClient.ApiCarrinhoByIdGetAsync(usuarioInput.UsuarioId);
            PrintCarrinho(carrinhoCliente);

            System.Console.WriteLine("ApiCarrinhoCheckoutPostAsync...");
            Guid guid = Guid.NewGuid();
            await carrinhoClient.ApiCarrinhoCheckoutPostAsync(carrinhoCliente, guid.ToString());
        }

        private static void PrintCarrinho(CarrinhoCliente carrinhoCliente)
        {
            foreach (var item in carrinhoCliente.Items)
            {
                System.Console.WriteLine(
                    $"Id: {item.Id}, PrecoUnitario: {item.PrecoUnitario}, ProdutoId: {item.ProdutoId}, ProdutoNome: {item.ProdutoNome}, Quantidade: {item.Quantidade}, UrlImagem: {item.UrlImagem}"
                );
            }
        }
    }
}