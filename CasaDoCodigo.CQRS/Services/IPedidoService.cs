using CasaDoCodigo.OrdemDeCompra.Models.DTOs;
using CasaDoCodigo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Services
{
    public interface IPedidoService : IService
    {
        Task<List<PedidoDTO>> GetAsync(string clienteId);
    }
}
