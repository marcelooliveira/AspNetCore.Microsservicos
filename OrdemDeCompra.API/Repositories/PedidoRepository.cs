using CasaDoCodigo.OdemDeCompra;
using CasaDoCodigo.OrdemDeCompra.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        public PedidoRepository(ApplicationContext contexto) : base(contexto)
        {
        }

        public async Task<Pedido> CreateOrUpdate(Pedido pedido)
        {
            EntityEntry<Pedido> entityEntry;
            entityEntry = await dbSet.AddAsync(pedido);
            await contexto.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
