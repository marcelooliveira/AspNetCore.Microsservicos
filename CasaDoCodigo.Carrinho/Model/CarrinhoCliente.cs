using System.Collections.Generic;

namespace CasaDoCodigo.Carrinho.Model
{
    public class CarrinhoCliente
    {
        public string ClienteId { get; set; }
        public List<ItemCarrinho> Items { get; set; }
    }
}