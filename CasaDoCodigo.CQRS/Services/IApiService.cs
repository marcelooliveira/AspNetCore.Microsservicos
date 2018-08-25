using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Services
{
    public interface IApiService
    {
        Task<IEnumerable<Produto>> GetProdutos();
    }
}
