using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Commands
{
    public class CreatePedidoCommand
        : IRequest<bool>
    {
        public CreatePedidoCommand()
        {

        }

        public CreatePedidoCommand(List<CreatePedidoCommandItem> itens, string clienteId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            Itens = itens;
            ClienteId = clienteId;
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

        public List<CreatePedidoCommandItem> Itens { get; private set; } = new List<CreatePedidoCommandItem>();
        public string ClienteId { get; set; } = "";
        public string ClienteNome { get; set; } = "";
        public string ClienteEmail { get; set; } = "";
        public string ClienteTelefone { get; set; } = "";
        public string ClienteEndereco { get; set; } = "";
        public string ClienteComplemento { get; set; } = "";
        public string ClienteBairro { get; set; } = "";
        public string ClienteMunicipio { get; set; } = "";
        public string ClienteUF { get; set; } = "";
        public string ClienteCEP { get; set; } = "";
    }

    public class CreatePedidoCommandItem
    {
        public CreatePedidoCommand Pedido { get; set; }
        public string ProdutoCodigo { get; set; }
        public string ProdutoNome { get; set; }
        public decimal ProdutoPreco { get; set; }
        public int ProdutoQuantidade { get; set; }
        public decimal ProdutoPrecoUnitario { get; set; }
        public decimal Subtotal => ProdutoQuantidade * ProdutoPrecoUnitario;

        public CreatePedidoCommandItem()
        {

        }

        public CreatePedidoCommandItem(string produtoCodigo, string produtoNome, decimal produtoPreco, int produtoQuantidade, decimal produtoPrecoUnitario)
        {
            ProdutoCodigo = produtoCodigo;
            ProdutoNome = produtoNome;
            ProdutoPreco = produtoPreco;
            ProdutoQuantidade = produtoQuantidade;
            ProdutoPrecoUnitario = produtoPrecoUnitario;
        }

        public void AtualizaQuantidade(int produtoQuantidade)
        {
            ProdutoQuantidade = produtoQuantidade;
        }
    }

}
