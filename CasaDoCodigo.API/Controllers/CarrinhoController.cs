using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : BaseApiController
    {
        private readonly IPedidoRepository pedidoRepository;

        public CarrinhoController(ILogger<CarrinhoController> logger,
            IPedidoRepository pedidoRepository) : base(logger)
        {
            this.pedidoRepository = pedidoRepository;
        }

        /// <summary>
        /// Obtém o carrinho de compras.
        /// </summary>
        /// <param name="pedidoId">O id do pedido</param>
        /// <param name="codigo">O código do produto a ser inserido, ou vazio para não inserir nenhum produto</param>
        /// <returns>O carrinho de compras</returns>
        /// <response code="400">Código de produto inválido</response> 
        /// <response code="404">Pedido não encontrado</response> 
        [HttpGet("{pedidoId}/{codigo}", Name = "GetCarrinho")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCarrinho(int pedidoId, string codigo)
        {
            Pedido pedido = await pedidoRepository.GetPedido(pedidoId);
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest($"Código inválido: {codigo}");
            }

            if (pedido == null)
            {
                return NotFound($"Pedido não encontrado com id: {pedidoId}");
            }

            await pedidoRepository.AddItem(pedido.Id, codigo);

            pedido = await pedidoRepository.GetPedido(pedido.Id);
            return Ok(new CarrinhoViewModel(pedido.Id, pedido.Itens));
        }

        /// <summary>
        /// Atualiza a quantidade de produtos de um item do carrinho.
        /// </summary>
        /// <param name="input">Os dados do item sendo atualizado</param>
        /// <returns>O item do carrinho atualizado</returns>
        /// <response code="400">O item do carrinho não foi encontrado</response> 
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]UpdateQuantidadeInput input)
        {
            if (input.ItemPedidoId <= 0)
            {
                return BadRequest($"Id o item de pedido inválido: {input.ItemPedidoId}");
            }

            return Ok(await pedidoRepository.UpdateQuantidade(input));
        }
    }
}
