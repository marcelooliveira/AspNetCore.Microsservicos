using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models
{
    public class ProdutoViewModel
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Id { get; set; }
    }

    public class Item
    {
        public ProdutoViewModel Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
        public int Id { get; set; }
    }

    public class CadastroViewModel
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        [MinLength(5, ErrorMessage = "Nome deve ter no mínimo 5 caracteres")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = "";
        [Required(ErrorMessage = "Email é obrigatório")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Telefone { get; set; } = "";
        [Required(ErrorMessage = "Endereco é obrigatório")]
        public string Endereco { get; set; } = "";
        [Required(ErrorMessage = "Complemento é obrigatório")]
        public string Complemento { get; set; } = "";
        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string Bairro { get; set; } = "";
        [Required(ErrorMessage = "Municipio é obrigatório")]
        public string Municipio { get; set; } = "";
        [Required(ErrorMessage = "UF é obrigatório")]
        public string UF { get; set; } = "";
        [Required(ErrorMessage = "CEP é obrigatório")]
        public string CEP { get; set; } = "";

        public CadastroViewModel()
        {

        }

        public CadastroViewModel(Cadastro cadastro)
        {
            this.Id = cadastro.Id;
            this.Bairro = cadastro.Bairro;
            this.CEP = cadastro.CEP;
            this.Complemento = cadastro.Complemento;
            this.Email = cadastro.Email;
            this.Endereco = cadastro.Endereco;
            this.Municipio = cadastro.Municipio;
            this.Nome = cadastro.Nome;
            this.Telefone = cadastro.Telefone;
            this.UF = cadastro.UF;
        }
    }

    public class PedidoViewModel
    {
        public List<Item> Itens { get; set; }
        public int CadastroId { get; set; }
        public CadastroViewModel Cadastro { get; set; }
        public int Id { get; set; }
    }
}
