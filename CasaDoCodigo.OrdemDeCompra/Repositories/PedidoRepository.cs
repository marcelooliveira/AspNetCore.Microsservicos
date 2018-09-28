using CasaDoCodigo.OrdemDeCompra.Models;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public interface IPedidoRepository
    {
        Task<bool> CreateOrUpdate(Pedido pedido);
    }

    public class PedidoRepository : IPedidoRepository
    {
        public Task<bool> CreateOrUpdate(Pedido pedido)
        {
            return new Task<bool>(() => true);
        }
    }
}
