using CasaDoCodigo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface ICarrinhoService
    {
        Task<CarrinhoViewModel> GetCarrinho(string userId);
        Task<CarrinhoViewModel> AddItem(string clienteId, ItemCarrinho input);
        Task<CarrinhoViewModel> UpdateItem(string clienteId, ItemCarrinho input);
    }
}
