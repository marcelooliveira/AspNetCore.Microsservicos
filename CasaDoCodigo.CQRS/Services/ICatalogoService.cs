using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface ICatalogoService
    {
        Task<IEnumerable<Produto>> GetProdutos();
        Task<Produto> GetProduto(string codigo);
    }
}
