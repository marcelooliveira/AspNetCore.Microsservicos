using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.OrdemDeCompra.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Mensagens.EventHandling
{
    public class CheckoutEventHandler : IHandleMessages<CheckoutAceitoEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePedidoCommandHandler> _logger;

        public CheckoutEventHandler(IMediator mediator, ILogger<CreatePedidoCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        Task IHandleMessages<CheckoutAceitoEvent>.Handle(CheckoutAceitoEvent message)
        {
            LogMessage(message);
            SendCommand(message);
            return Task.CompletedTask;
        }

        private void SendCommand(CheckoutAceitoEvent message)
        {
            try
            {
                var itens = message.Itens.Select(
                        i => new CreatePedidoCommandItem(i.ProdutoId, i.ProdutoNome, i.PrecoUnitario, i.Quantidade, i.PrecoUnitario)
                    ).ToList();

                var createPedidoCommand = new CreatePedidoCommand(itens, message.UserId, message.UserName, message.Email, message.Fone, message.Endereco, message.Complemento, message.Bairro, message.Municipio, message.UF, message.Cep);

                var requestCreateOrder = new IdentifiedCommand<CreatePedidoCommand, bool>(createPedidoCommand, message.Id);

                //HACK: I should never call Wait(), but this method override cannot be async...
                _mediator.Send(requestCreateOrder);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private void LogMessage(CheckoutAceitoEvent message)
        {
            _logger.LogInformation("Received Checkout. Message Follows");
            _logger.LogInformation("----------------------------------");
            _logger.LogInformation(message.UserId);
            foreach (var item in message.Itens)
            {
                _logger.LogInformation(
                $"Id = {item.Id}, " +
                $"ProdutoId = {item.ProdutoId}, " +
                $"ProdutoNome = {item.ProdutoNome}, " +
                $"PrecoUnitario = {item.PrecoUnitario}, " +
                $"Quantidade = {item.Quantidade}, " +
                $"UrlImagem = {item.UrlImagem}, ");
            }
            _logger.LogInformation("----------------------------------");
            _logger.LogInformation("Message Ends");
        }
    }


}
