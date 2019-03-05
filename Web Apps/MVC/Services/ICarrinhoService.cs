using MVC.Models;
using MVC.Models.ViewModels;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Services
{
    public interface ICarrinhoService : IService
    {
        Task<CarrinhoCliente> GetCarrinho(string userId);
        Task<CarrinhoCliente> AddItem(string clienteId, ItemCarrinho input);
        Task<UpdateQuantidadeOutput> UpdateItem(string clienteId, UpdateQuantidadeInput input);
        Task<CarrinhoCliente> DefinirQuantidades(ApplicationUser applicationUser, Dictionary<string, int> quantidades);
        Task AtualizarCarrinho(CarrinhoCliente carrinhoCliente);
        Task<bool> Checkout(string clienteId, CadastroViewModel viewModel);
    }
}
