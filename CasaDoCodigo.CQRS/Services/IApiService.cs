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
        Task<PedidoViewModel> GetPedido(int pedidoId);
        Task<PedidoViewModel> UpdateCadastro(CadastroViewModel viewModel);
        Task<UpdateQuantidadeOutput> UpdateQuantidade(int itemPedidoId, int quantidade);
    }
}
