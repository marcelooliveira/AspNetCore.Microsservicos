using System.Collections.Generic;
using System.Threading.Tasks;
using CasaDoCodigo.Models;

namespace CasaDoCodigo.API.Queries
{
    public interface IProdutoQueries
    {
        Task<IEnumerable<Produto>> GetProdutosAsync();
        Task<Produto> GetProdutoAsync(string codigo);
    }
}