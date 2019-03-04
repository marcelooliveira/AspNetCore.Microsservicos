using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.Mensagens.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.Mensagens.IntegrationEvents
{
    public abstract class BaseEventHandler<TMessage, TCommand> 
        where TMessage: IntegrationEvent
        where TCommand: IRequest<bool>
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger _logger;

        public BaseEventHandler(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public Task Handle(TMessage message)
        {
            LogMessage(message);
            SendCommand(message);
            return Task.CompletedTask;
        }

        void SendCommand(TMessage message)
        {
            try
            {
                TCommand command = GetCommand(message);

                var request = new IdentifiedCommand<TCommand, bool>(command, message.Id);

                _mediator.Send(request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        protected abstract TCommand GetCommand(TMessage message);

        void LogMessage(TMessage message)
        {
            _logger.LogInformation("Message received:");
            _logger.LogInformation("----------------------------------");
            _logger.LogInformation(JsonConvert.SerializeObject(message));
            _logger.LogInformation("----------------------------------");
        }
    }
}
