using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System.Diagnostics;

namespace CasaDoCodigo.OdemDeCompra.IntegrationEvents.EventHandling
{
    public class CheckoutEventHandler : Paramore.Brighter.RequestHandler<CheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILoggerFactory _logger;
        //private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public override CheckoutEvent Handle(CheckoutEvent @event)
        {
            Trace.WriteLine("Received Checkout. Message Follows");
            Trace.WriteLine("----------------------------------");
            Trace.WriteLine(@event.ClienteId);
            foreach (var item in @event.Items)
            {
                Trace.WriteLine(
                $"Id = {item.Id}, " +
                $"ProdutoId = {item.ProdutoId}, " +
                $"ProdutoNome = {item.ProdutoNome}, " +
                $"PrecoUnitario = {item.PrecoUnitario}, " +
                $"Quantidade = {item.Quantidade}, " +
                $"UrlImagem = {item.UrlImagem}, ");
            }
            Trace.WriteLine("----------------------------------");
            Trace.WriteLine("Message Ends");
            return base.Handle(@event);
        }
    }
}
