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
        public Task<bool> Handle(IdentifiedCommand<CreatePedidoCommand, bool> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
