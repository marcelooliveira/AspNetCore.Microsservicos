using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Commands
{
    [DataContract]
    public class CreatePedidoCommand
        : IRequest<bool>
    {
        public CreatePedidoCommand()
        {
        }
    }
}
