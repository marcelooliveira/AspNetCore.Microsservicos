using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models.ViewModels
{
    public class CarrinhoViewModel
    {
        public CarrinhoViewModel(string clienteId, IList<ItemCarrinho> itens)
        {
            ClienteId = clienteId;
            Itens = itens;
        }

        public string ClienteId { get; set; }
        public IList<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();
        public decimal Total => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    }

    public class ItemCarrinho 
    {
        public ItemCarrinho()
        {

        }

        public ItemCarrinho(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade, string urlImagem)
        {   
            Id = id;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
            UrlImagem = urlImagem;
        }

        public string Id { get; set; }
        public string ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
        public string UrlImagem { get; set; }
    }
}
