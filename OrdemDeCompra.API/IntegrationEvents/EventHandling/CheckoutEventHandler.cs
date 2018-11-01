using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.OrdemDeCompra.Commands;
using MediatR;
using Rebus.Handlers;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Mensagens.EventHandling
{
    public class CheckoutEventHandler : IHandleMessages<CheckoutAceitoEvent>
    {
        private readonly IMediator _mediator;

        public CheckoutEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        Task IHandleMessages<CheckoutAceitoEvent>.Handle(CheckoutAceitoEvent message)
        {
            Trace.WriteLine("Received Checkout. Message Follows");
            Trace.WriteLine("----------------------------------");
            Trace.WriteLine(message.UserId);
            foreach (var item in message.Itens)
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

            var itens = message.Itens.Select(
                    i => new CreatePedidoCommandItem(i.ProdutoId, i.ProdutoNome, i.PrecoUnitario, i.Quantidade, i.PrecoUnitario)
                ).ToList();

            var createPedidoCommand = new CreatePedidoCommand(itens, message.UserId, message.UserName, message.Email, message.Fone, message.Endereco, message.Complemento, message.Bairro, message.Municipio, message.UF, message.Cep);

            var requestCreateOrder = new IdentifiedCommand<CreatePedidoCommand, bool>(createPedidoCommand, message.Id);

            //HACK: I should never call Wait(), but this method override cannot be async...
            _mediator.Send(requestCreateOrder);
            return Task.CompletedTask;
        }
    }


}
