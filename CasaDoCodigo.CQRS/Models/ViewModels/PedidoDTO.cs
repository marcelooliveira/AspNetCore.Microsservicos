using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Models.ViewModels
{
    public class PedidoDTO
    {
        public PedidoDTO()
        {

        }

        public PedidoDTO(List<ItemPedidoDTO> itens, string id, string nome, string email, string telefone, string endereco, string complemento, string bairro, string municipio, string uF, string cEP)
        {
            Itens = itens;
            Id = id;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            Complemento = complemento;
            Bairro = bairro;
            Municipio = municipio;
            UF = uF;
            CEP = cEP;
        }

        public List<ItemPedidoDTO> Itens { get; set; } = new List<ItemPedidoDTO>();
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }

        public decimal Total => Itens.Sum(i => i.Subtotal);
    }
}
