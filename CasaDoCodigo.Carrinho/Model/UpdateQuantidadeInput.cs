using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Carrinho.API.Model
{
    public class UpdateQuantidadeInput
    {
        [Required]
        public string ProdutoId { get; set; }
        [Required]
        public int Quantidade { get; set; }
    }
}
