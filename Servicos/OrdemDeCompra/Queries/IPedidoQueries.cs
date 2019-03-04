using CasaDoCodigo.OrdemDeCompra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdemDeCompra.API.Queries
{
    public interface IPedidoQueries
    {
        IList<Pedido> GetPedidos(string clienteId);
    }
}
