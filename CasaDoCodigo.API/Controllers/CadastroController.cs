using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.API.Model;
using CasaDoCodigo.Models;
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
    public class CadastroController : BaseApiController
    {
        private readonly IPedidoRepository pedidoRepository;

        public CadastroController(ILogger<CadastroController> logger, IPedidoRepository pedidoRepository) : base(logger)
        {
            this.pedidoRepository = pedidoRepository;
        }

        /// <summary>
        /// Cria informações do cadastro do usuário.
        /// </summary>
        /// <param name="cadastroViewModel"></param>
        /// <returns>O pedido com o cadastro atualizado</returns>
        /// <response code="400">Id do pedido é inválido</response> 
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] CadastroViewModel cadastroViewModel)
        {
            if (cadastroViewModel.PedidoId == 0)
            {
                return BadRequest("Id do pedido é inválido");
            }

            var cadastro = new Cadastro(cadastroViewModel);
            var pedido = await pedidoRepository.UpdateCadastro(cadastroViewModel.PedidoId, cadastro);
            PedidoViewModel viewModel = new PedidoViewModel(pedido);
            return Ok(viewModel);
        }
    }
}
