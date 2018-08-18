using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
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
        /// <response code="201">Retorna o pedido atualizado</response>
        /// <response code="404">Pedido não encontrado ou produto não encontrado</response> 
        [HttpGet("{pedidoId}/{codigo}", Name = "GetCarrinho")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCarrinho(int pedidoId, string codigo)
        {
            Pedido pedido = await pedidoRepository.GetPedido(pedidoId);
            if (pedido == null)
            {
                return BadRequest($"Pedido não encontrado com id: {pedidoId}");
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest($"Código inválido: {codigo}");
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
        /// <response code="201">O item foi atualizado</response>
        /// <response code="404">O item do carrinho não foi encontrado</response> 
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
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
