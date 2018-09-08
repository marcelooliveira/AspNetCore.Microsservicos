using NServiceBus;
using System;
using System.Collections.Generic;

namespace CasaDoCodigo.Mensagens
{
    public class MensagemCarrinho : IEvent
    {
        public MensagemCarrinho()
        {

        }

        public MensagemCarrinho(string clienteId, List<MensagemItemCarrinho> items)
        {
            ClienteId = clienteId;
            Items = items;
        }

        public string ClienteId { get; set; }
        public IList<MensagemItemCarrinho> Items { get; set; }
    }

    public class MensagemItemCarrinho
    {
        public MensagemItemCarrinho()
        {

        }

        public MensagemItemCarrinho(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade, string urlImagem)
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
