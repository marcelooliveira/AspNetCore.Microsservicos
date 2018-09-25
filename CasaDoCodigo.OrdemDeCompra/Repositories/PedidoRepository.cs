using CasaDoCodigo.OrdemDeCompra.Models;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> CreateOrUpdate(Pedido pedido);
    }

    public class PedidoRepository : IPedidoRepository
    {
        public Task<Pedido> CreateOrUpdate(Pedido pedido)
        {
            throw new NotImplementedException();
        }
    }
}
