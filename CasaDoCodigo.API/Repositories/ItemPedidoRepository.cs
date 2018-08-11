using CasaDoCodigo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface IItemPedidoRepository
    {
        Task<ItemPedido> GetItemPedido(int itemPedidoId);
        Task RemoveItemPedido(int itemPedidoId);
    }

    public class ItemPedidoRepository : BaseRepository<ItemPedido>, IItemPedidoRepository
    {
        public ItemPedidoRepository(ApplicationContext contexto) : base(contexto)
        {
        }

        public async Task<ItemPedido> GetItemPedido(int itemPedidoId)
        {
            return
                await dbSet
                .Where(ip => ip.Id == itemPedidoId)
                .SingleOrDefaultAsync();
        }

        public async Task RemoveItemPedido(int itemPedidoId)
        {
            dbSet.Remove(await GetItemPedido(itemPedidoId));
        }
    }
}
