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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await pedidoRepository.CreateOrUpdate(pedido);
            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult> Get(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
            {
                return BadRequest();
            }

            IList<Pedido> pedidos = await pedidoRepository.GetPedidos(clienteId);

            if (pedidos == null)
            {
                return NotFound(clienteId);
            }

            return base.Ok(pedidos);
        }
    }
}
