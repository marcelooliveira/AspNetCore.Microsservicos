using CasaDoCodigo.OrdemDeCompra;
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
            if (pedido == null)
                throw new ArgumentNullException();

            EntityEntry<Pedido> entityEntry;
            try
            {
                entityEntry = await dbSet.AddAsync(pedido);
                await contexto.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return entityEntry.Entity;
        }
    }
}
