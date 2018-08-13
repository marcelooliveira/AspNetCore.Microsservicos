using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.API.Model;
using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CadastroController : BaseApiController
    {
        private readonly IPedidoRepository pedidoRepository;

        public CadastroController(ILogger<CadastroController> logger, IPedidoRepository pedidoRepository) : base(logger)
        {
            this.pedidoRepository = pedidoRepository;
        }

        [HttpPost]
        public async Task<PedidoViewModel> Post([FromBody] CadastroViewModel cadastroViewModel)
        {
            try
            {
                var cadastro = new Cadastro(cadastroViewModel);
                var pedido = await pedidoRepository.UpdateCadastro(cadastroViewModel.PedidoId, cadastro);
                PedidoViewModel viewModel = new PedidoViewModel(pedido);
                return viewModel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, "PostCadastro");
                throw;
            }
        }
    }
}
