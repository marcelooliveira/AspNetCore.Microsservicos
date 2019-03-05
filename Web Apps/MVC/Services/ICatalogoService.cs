using MVC.Models;
using MVC.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC.Services
{
    public interface ICatalogoService : IService
    {
        Task<IList<Produto>> GetProdutos();
        Task<IList<Produto>> BuscaProdutos(string pesquisa);
        Task<Produto> GetProduto(string codigo);
    }
}
