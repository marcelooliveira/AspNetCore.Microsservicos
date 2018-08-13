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
            IPedidoRepository pedidoRepository): base(logger)
        {
            this.pedidoRepository = pedidoRepository;
        }

        [HttpGet("{pedidoId}/{codigo}", Name = "GetCarrinho")]
        public async Task<CarrinhoViewModel> GetCarrinho(int pedidoId, string codigo)
        {
            try
            {
                Pedido pedido = await pedidoRepository.GetPedido(pedidoId);

                if (!string.IsNullOrEmpty(codigo))
                {
                    await pedidoRepository.AddItem(pedido.Id, codigo);
                }

                pedido = await pedidoRepository.GetPedido(pedido.Id);
                return new CarrinhoViewModel(pedido.Id, pedido.Itens);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "GetCarrinho");
                throw;
            }

        }

        [HttpPost]
        public async Task<UpdateQuantidadeOutput> Post([FromBody]UpdateQuantidadeInput input)
        {
            try
            {
                return await pedidoRepository.UpdateQuantidade(input);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "UpdateQuantidade");
                throw;
            }
        }
    }
}
