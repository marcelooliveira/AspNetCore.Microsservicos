using CasaDoCodigo.Mensagens.Events;
using Rebus.Handlers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CasaDoCodigo.Mensagens.EventHandling
{
    public class CheckoutEventHandler : IHandleMessages<CheckoutEvent>
    {
        Task IHandleMessages<CheckoutEvent>.Handle(CheckoutEvent message)
        {
            Trace.WriteLine("Received Checkout. Message Follows");
            Trace.WriteLine("----------------------------------");
            Trace.WriteLine(message.ClienteId);
            foreach (var item in message.Items)
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
            return Task.CompletedTask;
        }
    }
}
