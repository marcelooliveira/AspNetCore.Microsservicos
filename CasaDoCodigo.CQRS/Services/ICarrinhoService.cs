using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface ICarrinhoService
    {
        Task<CarrinhoCliente> GetCarrinho(string userId);
        Task<CarrinhoCliente> AddItem(string clienteId, ItemCarrinho input);
        Task<UpdateQuantidadeOutput> UpdateItem(string clienteId, ItemCarrinho input);
    }
}
