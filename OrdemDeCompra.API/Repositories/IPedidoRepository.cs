using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> CreateOrUpdate(Pedido pedido);
        Task<IList<Pedido>> GetPedidos(string clienteId);
    }
}
