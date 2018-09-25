using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Events;
using CasaDoCodigo.OrdemDeCompra.Commands;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CasaDoCodigo.OdemDeCompra.IntegrationEvents.EventHandling
{
    public class CheckoutEventHandler : Paramore.Brighter.RequestHandler<CheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILoggerFactory _logger;
        private readonly IPedidoRepository _pedidoRepository;

        public CheckoutEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

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

            var createPedidoCommand = new CreatePedidoCommand();

            var requestCreateOrder = new IdentifiedCommand<CreatePedidoCommand, bool>(createPedidoCommand, @event.Id);

            //HACK: I should never call Wait(), but this method override cannot be async...
            _mediator.Send(requestCreateOrder).Wait();

            return base.Handle(@event);
        }
    }
}
