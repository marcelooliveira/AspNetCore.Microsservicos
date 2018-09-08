using System.Collections.Generic;

namespace CasaDoCodigo.OdemDeCompra.Model
{
    public class CarrinhoCliente
    {
        public string ClienteId { get; set; }
        public List<ItemCarrinho> Items { get; set; }
    }
}