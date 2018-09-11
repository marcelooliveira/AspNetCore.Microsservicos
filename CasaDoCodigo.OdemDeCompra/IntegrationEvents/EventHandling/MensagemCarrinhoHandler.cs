using CasaDoCodigo.Mensagens.Ports.Commands;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace CasaDoCodigo.OdemDeCompra.IntegrationEvents.EventHandling
{
    public class MensagemCarrinhoHandler :
        IHandleMessages<CheckoutEvent>
    {
        static ILog log = LogManager.GetLogger<MensagemCarrinhoHandler>();

        public Task Handle(CheckoutEvent mensagem, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }
    }
}