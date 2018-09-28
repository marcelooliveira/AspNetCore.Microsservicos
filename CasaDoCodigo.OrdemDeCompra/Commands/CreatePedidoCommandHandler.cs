using CasaDoCodigo.OrdemDeCompra.Repositories;
using MediatR;
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
        private readonly IPedidoRepository pedidoRepository;

        public CreatePedidoCommandHandler(IPedidoRepository pedidoRepository)
        {
            this.pedidoRepository = pedidoRepository;
        }

        public async Task<bool> Handle(IdentifiedCommand<CreatePedidoCommand, bool> request, CancellationToken cancellationToken)
        {
            return await this.pedidoRepository.CreateOrUpdate(new Models.Pedido());
        }
    }
}
