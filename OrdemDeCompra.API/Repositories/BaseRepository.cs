using CasaDoCodigo.OrdemDeCompra;
using CasaDoCodigo.OrdemDeCompra.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public abstract class BaseRepository<T> where T : BaseModel
    {
        protected readonly DbContext contexto;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(DbContext contexto)
        {
            this.contexto = contexto;
            dbSet = contexto.Set<T>();
        }
    }
}
