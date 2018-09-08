using CasaDoCodigo.Mensagens;
using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Events;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.OdemDeCompra.IntegrationEvents.EventHandling
{
    public class MensagemCarrinhoHandler :
        IHandleMessages<MensagemCarrinho>
    {
        static ILog log = LogManager.GetLogger<MensagemCarrinhoHandler>();

        public Task Handle(MensagemCarrinho mensagem, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }
    }
}