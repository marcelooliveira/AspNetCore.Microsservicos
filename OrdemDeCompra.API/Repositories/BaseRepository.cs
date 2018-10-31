using CasaDoCodigo.OdemDeCompra;
using CasaDoCodigo.OrdemDeCompra.Models;
using Microsoft.EntityFrameworkCore;

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
}
