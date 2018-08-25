using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface IApiService
    {
        Task<IEnumerable<Produto>> GetProdutos();
        Task<CarrinhoViewModel> Carrinho(string codigo, int pedidoId);
    }
}
