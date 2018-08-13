using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.API.Model
{
    [DataContract]
    public class BaseViewModel
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class PedidoViewModel : BaseViewModel
    {
        public PedidoViewModel(Pedido pedido)
        {
            Id = pedido.Id;
            Itens = pedido.Itens.Select(i =>
                new ItemPedidoViewModel(i)
            ).ToArray();
            CadastroId = pedido.CadastroId;
            Cadastro = new CadastroViewModel(pedido.Cadastro);
        }

        [DataMember]
        public ItemPedidoViewModel[] Itens { get; set; }
        [DataMember]
        public int CadastroId { get; set; }
        [DataMember]
        public CadastroViewModel Cadastro { get; set; }
    }

    public class ItemPedidoViewModel : BaseViewModel
    {
        public ItemPedidoViewModel(ItemPedido itemPedido)
        {
            Id = itemPedido.Id;
            Produto = new ProdutoViewModel(itemPedido.Produto);
            Quantidade = itemPedido.Quantidade;
            PrecoUnitario = itemPedido.PrecoUnitario;
        }

        [DataMember]
        public ProdutoViewModel Produto { get; set; }
        [DataMember]
        public int Quantidade { get; set; }
        [DataMember]
        public decimal PrecoUnitario { get; set; }
        [DataMember]
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }

    public class CadastroViewModel : BaseViewModel
    {
        public CadastroViewModel(Cadastro cadastro)
        {
            Id = cadastro.Id;
            Nome = cadastro.Nome;
            Email = cadastro.Email;
            Telefone = cadastro.Telefone;
            Endereco = cadastro.Endereco;
            Complemento = cadastro.Complemento;
            Bairro = cadastro.Bairro;
            Municipio = cadastro.Municipio;
            UF = cadastro.UF;
            CEP = cadastro.CEP;
        }

        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Telefone { get; set; }
        [DataMember]
        public string Endereco { get; set; }
        [DataMember]
        public string Complemento { get; set; }
        [DataMember]
        public string Bairro { get; set; }
        [DataMember]
        public string Municipio { get; set; }
        [DataMember]
        public string UF { get; set; }
        [DataMember]
        public string CEP { get; set; }
    }

    public class ProdutoViewModel : BaseViewModel
    {
        public ProdutoViewModel(Produto produto)
        {
            Id = produto.Id;
            Codigo = produto.Codigo;
            Nome = produto.Nome;
            Preco = produto.Preco;
        }

        [DataMember]
        public string Codigo { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public decimal Preco { get; set; }
    }
}
