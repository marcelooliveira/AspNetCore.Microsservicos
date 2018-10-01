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
    }
}
