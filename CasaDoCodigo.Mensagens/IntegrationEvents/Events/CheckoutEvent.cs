using System;
using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Mensagens.Events
{
    public class CheckoutEvent : IntegrationEvent
    {
        public CheckoutEvent()
        {

        }

        public CheckoutEvent(
              string userId, string userName, string email, string fone
            , string endereco, string complemento, string bairro
            , string municipio, string uf, string cep
            , Guid requestId
            , IList<CheckoutEventItem> itens)
        {
            UserId = userId;
            UserName = userName;
            Municipio = municipio;
            Email = email;
            Fone = fone;
            Endereco = endereco;
            Complemento = complemento;
            Bairro = bairro;
            UF = uf;
            Cep = cep;
            RequestId = requestId;
            Itens = 
                itens
                    .Select(i => 
                        new CheckoutEventItem(
                            i.Id, 
                            i.ProdutoId, 
                            i.ProdutoNome, 
                            i.PrecoUnitario, 
                            i.Quantidade)).ToList();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public int PedidoId { get; set; }
        public string Municipio { get; set; }
        public string Email { get; set; }
        public string Fone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public Guid RequestId { get; set; }
        public List<CheckoutEventItem> Itens { get; } = new List<CheckoutEventItem>();
    }

    public class CheckoutEventItem
    {
        public CheckoutEventItem()
        {

        }

        public CheckoutEventItem(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade)
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
