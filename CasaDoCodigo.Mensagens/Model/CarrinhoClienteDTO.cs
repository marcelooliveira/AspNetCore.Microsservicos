using System.Collections.Generic;

namespace CasaDoCodigo.Mensagens.Model
{
    public class CarrinhoClienteDTO
    {
        public CarrinhoClienteDTO()
        {
        }

        public CarrinhoClienteDTO(string clienteId, List<ItemCarrinhoDTO> itens)
        {
            ClienteId = clienteId;
            Itens = itens;
        }

        public string ClienteId { get; set; }
        public List<ItemCarrinhoDTO> Itens { get; set; }
    }
}
