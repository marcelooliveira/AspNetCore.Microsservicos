using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents;
using CasaDoCodigo.OrdemDeCompra.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Mensagens.EventHandling
{
    public class CheckoutEventHandler : BaseEventHandler<CheckoutEvent, CreatePedidoCommand>, IHandleMessages<CheckoutEvent>
    {
        public CheckoutEventHandler(IMediator mediator, ILogger<CheckoutEventHandler> logger)
            : base(mediator, logger)
        {
        }

        protected override CreatePedidoCommand GetCommand(CheckoutEvent message)
        {
            var itens = message.Itens.Select(
                    i => new CreatePedidoCommandItem(i.ProdutoId, i.ProdutoNome, i.Quantidade, i.PrecoUnitario)
                ).ToList();

            var command = new CreatePedidoCommand(itens, message.UserId, message.UserName, message.Email, message.Fone, message.Endereco, message.Complemento, message.Bairro, message.Municipio, message.UF, message.Cep);
            return command;
        }
    }
}
