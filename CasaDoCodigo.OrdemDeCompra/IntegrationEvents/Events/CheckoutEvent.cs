using Paramore.Brighter;
using System;
using System.Collections.Generic;

namespace CasaDoCodigo.Mensagens.Events
{
    public class CheckoutEvent
    {
        public CheckoutEvent()
        {
            
        }

        public CheckoutEvent(string clienteId, List<CheckoutItem> items)
        {
            ClienteId = clienteId;
            Items = items;
        }

        public string ClienteId { get; set; }
        public IList<CheckoutItem> Items { get; set; }
        public Guid Id { get; internal set; }
    }

    public class CheckoutItem
    {
        public CheckoutItem()
        {

        }

        public CheckoutItem(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade, string urlImagem)
        {
            Id = id;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
            UrlImagem = urlImagem;
        }

        public string Id { get; set; }
        public string ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public string UrlImagem { get; set; }
    }
}
