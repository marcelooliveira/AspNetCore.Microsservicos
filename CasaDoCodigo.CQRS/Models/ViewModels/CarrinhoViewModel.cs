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
            Items = itens;
        }

        public string ClienteId { get; set; }
        public IList<ItemCarrinho> Items { get; set; }
        public decimal Total => Items.Sum(i => i.Quantidade * i.PrecoUnitario);
    }

    public class ItemCarrinho 
    {
        public string Id { get; set; }
        public string ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public string UrlImagem { get; set; }
    }
}
