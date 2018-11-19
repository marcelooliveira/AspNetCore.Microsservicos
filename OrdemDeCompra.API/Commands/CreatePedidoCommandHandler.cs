using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Commands
{
    public class CreatePedidoCommandHandler
        : IRequestHandler<IdentifiedCommand<CreatePedidoCommand, bool>, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<CreatePedidoCommandHandler> _logger;
        private readonly IBus _bus;

        public CreatePedidoCommandHandler(
            ILogger<CreatePedidoCommandHandler> logger
            , IPedidoRepository pedidoRepository
            , IBus bus)
        {
            this._pedidoRepository = pedidoRepository;
            this._logger = logger;
            this._bus = bus;
        }

        public async Task<bool> Handle(IdentifiedCommand<CreatePedidoCommand, bool> request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException("Request cannot be empty");

            var cmd = request.Command;
            var guid = request.Id;

            if (cmd == null)
                throw new ArgumentNullException("Command cannot be empty");

            if (guid == Guid.Empty)
                throw new ArgumentException("Guid cannot be empty");

            var itens = cmd.Itens.Select(
                    i => new ItemPedido(i.ProdutoCodigo, i.ProdutoNome, i.ProdutoQuantidade, i.ProdutoPrecoUnitario)
                ).ToList();

            if (itens.Count == 0)
            {
                throw new NoItemsException();
            }


            foreach (var item in itens)
            {
                if (
                    string.IsNullOrWhiteSpace(item.ProdutoCodigo)
                    || string.IsNullOrWhiteSpace(item.ProdutoNome)
                    || item.ProdutoQuantidade <= 0
                    || item.ProdutoPrecoUnitario <= 0
                    )
                {
                    throw new InvalidItemException();
                }
            }


            if (string.IsNullOrWhiteSpace(cmd.ClienteId)
                 || string.IsNullOrWhiteSpace(cmd.ClienteNome)
                 || string.IsNullOrWhiteSpace(cmd.ClienteEmail)
                 || string.IsNullOrWhiteSpace(cmd.ClienteTelefone)
                 || string.IsNullOrWhiteSpace(cmd.ClienteEndereco)
                 || string.IsNullOrWhiteSpace(cmd.ClienteComplemento)
                 || string.IsNullOrWhiteSpace(cmd.ClienteBairro)
                 || string.IsNullOrWhiteSpace(cmd.ClienteMunicipio)
                 || string.IsNullOrWhiteSpace(cmd.ClienteUF)
                 || string.IsNullOrWhiteSpace(cmd.ClienteCEP)
                )
                throw new InvalidUserDataException();

            var pedido = new Pedido(itens, cmd.ClienteId,
                cmd.ClienteNome, cmd.ClienteEmail, cmd.ClienteTelefone, 
                cmd.ClienteEndereco, cmd.ClienteComplemento, cmd.ClienteBairro, 
                cmd.ClienteMunicipio, cmd.ClienteUF, cmd.ClienteCEP);

            try
            {
                await this._pedidoRepository.CreateOrUpdate(pedido);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
