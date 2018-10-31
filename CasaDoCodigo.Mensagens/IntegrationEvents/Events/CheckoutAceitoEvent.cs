using CasaDoCodigo.Mensagens.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Mensagens.Events
{
    public class CheckoutAceitoEvent : IntegrationEvent
    {
        public CheckoutAceitoEvent()
        {

        }

        public CheckoutAceitoEvent(
            string userId
            , string userName
            , string municipio
            , string endereco
            , string uf
            , string cep
            , Guid requestId
            , CarrinhoClienteDTO carrinho)
        {
            UserId = userId;
            UserName = userName;
            Municipio = municipio;
            Endereco = endereco;
            UF = uf;
            Cep = cep;
            RequestId = requestId;
            Itens = 
                carrinho
                    .Itens
                    .Select(i => 
                        new CheckoutAceitoEventItem(
                            i.Id, 
                            i.ProdutoId, 
                            i.ProdutoNome, 
                            i.PrecoUnitario, 
                            i.Quantidade)).ToList();
        }

        public string UserId { get; }
        public string UserName { get; }
        public int PedidoId { get; set; }
        public string Municipio { get; set; }
        public string Endereco { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public Guid RequestId { get; set; }
        public List<CheckoutAceitoEventItem> Itens { get; } = new List<CheckoutAceitoEventItem>();
    }

    public class CheckoutAceitoEventItem
    {
        public CheckoutAceitoEventItem()
        {

        }

        public CheckoutAceitoEventItem(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade)
        {
            Id = id;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
        }

        public string Id { get; set; }
        public string ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string UrlImagem { get; set; }
    }

}
