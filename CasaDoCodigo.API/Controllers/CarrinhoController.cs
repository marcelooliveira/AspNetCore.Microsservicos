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
    public class CarrinhoController : ControllerBase
    {
        private readonly IPedidoRepository pedidoRepository;
        private readonly ILogger logger;

        public CarrinhoController(ILogger<CarrinhoController> logger,
            IPedidoRepository pedidoRepository)
        {
            this.logger = logger;
            this.pedidoRepository = pedidoRepository;
        }

        [HttpGet("{codigo}", Name = "GetCarrinho")]
        public async Task<CarrinhoViewModel> GetCarrinho(string codigo)
        {
            try
            {
                if (!string.IsNullOrEmpty(codigo))
                {
                    await pedidoRepository.AddItem(codigo);
                }

                Pedido pedido = await pedidoRepository.GetPedido();
                return new CarrinhoViewModel(pedido.Itens);
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
