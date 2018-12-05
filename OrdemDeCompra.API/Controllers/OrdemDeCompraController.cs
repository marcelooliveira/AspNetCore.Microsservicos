using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDoCodigo.OrdemDeCompra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdemDeCompraController : ControllerBase
    {
        private readonly IPedidoRepository pedidoRepository;

        public object JwtClaimTypes { get; private set; }

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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string clienteId = GetUserId();

            IList<Pedido> pedidos = await pedidoRepository.GetPedidos(clienteId);

            if (pedidos == null)
            {
                return NotFound(clienteId);
            }

            return base.Ok(pedidos);
        }

        private string GetUserId()
        {
            var userIdClaim = 
                User
                .Claims
                .FirstOrDefault(x => new[] 
                    {
                        "sub", ClaimTypes.NameIdentifier
                    }.Contains(x.Type)
                && !string.IsNullOrWhiteSpace(x.Value));

            if (userIdClaim != null)
                return userIdClaim.Value;

            throw new InvalidUserDataException("Usuário desconhecido");
        }
    }
}
