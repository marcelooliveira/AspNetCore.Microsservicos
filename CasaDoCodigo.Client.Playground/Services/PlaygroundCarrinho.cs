using CasaDoCodigo.Client.Carrinho.Generated;
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
            await carrinhoClient.ApiCarrinhoPostAsync(new CarrinhoCliente
            {
                ClienteId = "xpto",
                Items = new ObservableCollection<ItemCarrinho>(new List<ItemCarrinho>
                {
                    new ItemCarrinho { Id = "1", PrecoUnitario = 12.34, ProdutoId = "1", ProdutoNome = "Java para baixinhos", Quantidade = 2, UrlImagem = "http:\\img1.images.com}" },
                    new ItemCarrinho { Id = "2", PrecoUnitario = 23.45, ProdutoId = "2", ProdutoNome = "C# for dummies", Quantidade = 3, UrlImagem = "http:\\img2.images.com}" }
                })
            });
        }
    }
}