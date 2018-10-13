using System.Collections.Generic;

namespace CasaDoCodigo.Carrinho.Model
{
    public class CarrinhoCliente
    {
        public CarrinhoCliente()
        {
        }

        public CarrinhoCliente(string clienteId)
        {
            ClienteId = clienteId;
            Itens = new List<ItemCarrinho>();
        }

        public string ClienteId { get; set; }
        public List<ItemCarrinho> Itens { get; set; }
    }
}