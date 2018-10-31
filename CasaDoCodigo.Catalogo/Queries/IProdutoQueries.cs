using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.API.Queries
{
    public interface IProdutoQueries
    {
        Task<IEnumerable<Produto>> GetProdutosAsync();
        Task<Produto> GetProdutoAsync(string codigo);
    }
}