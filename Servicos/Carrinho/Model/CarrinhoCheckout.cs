using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carrinho.API.Model
{
    public class CarrinhoCheckout
    {
        public string Municipio { get; set; }
        public string Email { get; set; }
        public string Fone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public string Cliente { get; set; }
        public Guid RequestId { get; set; }
    }
}
