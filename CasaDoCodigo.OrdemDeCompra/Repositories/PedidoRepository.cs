using CasaDoCodigo.OdemDeCompra;
using CasaDoCodigo.OrdemDeCompra.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public abstract class BaseRepository<T> where T : BaseModel
    {
        protected readonly ApplicationContext contexto;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(ApplicationContext contexto)
        {
            this.contexto = contexto;
            dbSet = contexto.Set<T>();
        }
    }

    public interface IPedidoRepository
    {
        Task<Pedido> CreateOrUpdate(Pedido pedido);
    }

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
