using CasaDoCodigo.OrdemDeCompra.Models;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> CreateOrUpdate(Pedido pedido);
    }
}
