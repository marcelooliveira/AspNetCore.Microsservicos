using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.API.Model
{
    public class PedidoViewModel
    {
        public int Id { get; set; }
        public List<ItemPedidoViewModel> Itens { get; set; } = new List<ItemPedidoViewModel>();
        public int CadastroId { get; set; }
        public CadastroViewModel Cadastro { get; set; }
    }

    public class ItemPedidoViewModel
    {
        public int Id { get; set; }
        public ProdutoViewModel Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }

    public class CadastroViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
    }

    public class ProdutoViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}
