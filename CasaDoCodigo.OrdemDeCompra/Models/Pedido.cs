using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasaDoCodigo.OrdemDeCompra.Models
{
    public class Pedido : BaseModel
    {
        public Pedido(List<ItemPedido> itens, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            Itens = itens;
            ClienteNome = clienteNome;
            ClienteEmail = clienteEmail;
            ClienteTelefone = clienteTelefone;
            ClienteEndereco = clienteEndereco;
            ClienteComplemento = clienteComplemento;
            ClienteBairro = clienteBairro;
            ClienteMunicipio = clienteMunicipio;
            ClienteUF = clienteUF;
            ClienteCEP = clienteCEP;
        }

        public List<ItemPedido> Itens { get; private set; } = new List<ItemPedido>();
        [MinLength(5, ErrorMessage = "Nome deve ter no mínimo 5 caracteres")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string ClienteNome { get; set; } = "";
        [Required(ErrorMessage = "Email é obrigatório")]
        public string ClienteEmail { get; set; } = "";
        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string ClienteTelefone { get; set; } = "";
        [Required(ErrorMessage = "Endereco é obrigatório")]
        public string ClienteEndereco { get; set; } = "";
        [Required(ErrorMessage = "Complemento é obrigatório")]
        public string ClienteComplemento { get; set; } = "";
        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string ClienteBairro { get; set; } = "";
        [Required(ErrorMessage = "Municipio é obrigatório")]
        public string ClienteMunicipio { get; set; } = "";
        [Required(ErrorMessage = "UF é obrigatório")]
        public string ClienteUF { get; set; } = "";
        [Required(ErrorMessage = "CEP é obrigatório")]
        public string ClienteCEP { get; set; } = "";
    }
}
