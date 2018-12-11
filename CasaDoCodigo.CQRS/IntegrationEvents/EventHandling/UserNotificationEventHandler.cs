using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using MVC.Commands;
using Rebus.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Mensagens.EventHandling
{
    public class UserNotificationEventHandler : BaseEventHandler<UserNotificationEvent, UserNotificationCommand>, IHandleMessages<UserNotificationEvent>
    {
        public UserNotificationEventHandler(IMediator mediator, ILogger<UserNotificationEventHandler> logger)
            : base(mediator, logger)
        {
        }

        protected override UserNotificationCommand GetCommand(UserNotificationEvent message)
        {
            return new UserNotificationCommand(message.UsuarioId, message.Mensagem, message.DateCreated);
        }
    }
}
