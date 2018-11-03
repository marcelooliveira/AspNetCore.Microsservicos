using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CasaDoCodigo.OrdemDeCompra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdemDeCompraController : ControllerBase
    {
        private readonly IPedidoRepository pedidoRepository;

        public OrdemDeCompraController(IPedidoRepository pedidoRepository)
        {
            this.pedidoRepository = pedidoRepository;
        }

        // POST api/ordemdecompra
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            var resultado = await pedidoRepository.CreateOrUpdate(pedido);
            return Ok(resultado);
        }
    }
}
