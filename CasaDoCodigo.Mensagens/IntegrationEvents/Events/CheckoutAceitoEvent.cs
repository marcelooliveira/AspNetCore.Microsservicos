using CasaDoCodigo.Mensagens.Model;
using System;

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
            , string cliente
            , Guid requestId
            , CarrinhoClienteDTO carrinho)
        {
            UserId = userId;
            UserName = userName;
            Municipio = municipio;
            Endereco = endereco;
            this.UF = uf;
            Cep = cep;
            Cliente = cliente;
            RequestId = requestId;
            Carrinho = carrinho;
        }

        public string UserId { get; }
        public string UserName { get; }
        public int PedidoId { get; set; }
        public string Municipio { get; set; }
        public string Endereco { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public string Cliente { get; set; }
        public Guid RequestId { get; set; }
        public CarrinhoClienteDTO Carrinho { get; }
    }
}
