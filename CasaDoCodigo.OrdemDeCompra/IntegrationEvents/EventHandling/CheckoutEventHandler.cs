using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Events;
using Paramore.Brighter;
using System.Diagnostics;

namespace CasaDoCodigo.OdemDeCompra.IntegrationEvents.EventHandling
{
    public class CheckoutEventHandler : RequestHandler<CheckoutEvent>
    {
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
