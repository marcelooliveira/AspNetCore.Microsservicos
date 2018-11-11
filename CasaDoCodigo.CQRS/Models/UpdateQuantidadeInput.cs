using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MVC.Models
{
    [DataContract]
    public class UpdateQuantidadeInput
    {
        public UpdateQuantidadeInput()
        {

        }

        public UpdateQuantidadeInput(string produtoId, int quantidade)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
        }

        [Required]
        public string ProdutoId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantidade { get; set; }
    }
}
