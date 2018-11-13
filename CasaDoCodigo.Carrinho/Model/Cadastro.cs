using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Carrinho.API.Model
{
    public class CadastroViewModel
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        [Required]
        public string Nome { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Complemento { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Municipio { get; set; } = "";
        public string UF { get; set; } = "";
        public string CEP { get; set; } = "";

        public CadastroViewModel()
        {

        }
    }
}
