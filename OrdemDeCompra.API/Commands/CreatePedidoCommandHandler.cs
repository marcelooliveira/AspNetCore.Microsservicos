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
            var cmd = request.Command;

            var itens = cmd.Itens.Select(
                    i => new ItemPedido(i.ProdutoCodigo, i.ProdutoNome, i.ProdutoPreco, i.ProdutoQuantidade, i.ProdutoPrecoUnitario)
                ).ToList();

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
