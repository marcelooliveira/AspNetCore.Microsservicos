using System.Collections.Generic;

namespace CasaDoCodigo.Mensagens.Model
{
    public interface ICarrinhoCliente
    {
        string ClienteId { get; set; }
        List<ItemCarrinhoDTO> Itens { get; set; }
        decimal Total { get; }
    }
}