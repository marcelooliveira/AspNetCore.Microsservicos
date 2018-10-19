using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Carrinho.Model
{
    public interface ICarrinhoRepository
    {
        Task<CarrinhoCliente> GetCarrinhoAsync(string clienteId);
        IEnumerable<string> GetUsuarios();
        Task<CarrinhoCliente> UpdateCarrinhoAsync(CarrinhoCliente carrinho);
        Task<CarrinhoCliente> AddCarrinhoAsync(string clienteId, ItemCarrinho item);
        Task<CarrinhoCliente> UpdateCarrinhoAsync(string clienteId, ItemCarrinho item);
        Task<bool> DeleteCarrinhoAsync(string id);
    }
}