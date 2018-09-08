using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CasaDoCodigo.Carrinho.IntegrationEvents;
using CasaDoCodigo.Carrinho.Model;
using CasaDoCodigo.Mensagens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;

namespace CasaDoCodigo.Carrinho.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoRepository _repository;
        private readonly IEndpointInstance _endpoint;

        public CarrinhoController(ICarrinhoRepository repository
            , IEndpointInstance endpoint)
        {
            _repository = repository;
            _endpoint = endpoint;
        }

        //GET /id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CarrinhoCliente), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var carrinho = await _repository.GetCarrinhoAsync(id);
            if (carrinho == null)
            {
                return Ok(new CarrinhoCliente() { ClienteId = id });
            }
            return Ok(carrinho);
        }

        //POST /value
        [HttpPost]
        [ProducesResponseType(typeof(CarrinhoCliente), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] CarrinhoCliente input)
        {
            var carrinho = await _repository.UpdateCarrinhoAsync(input);
            return Ok(carrinho);
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody]CarrinhoCliente carrinhoCliente, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var eventMessage 
                = new MensagemCarrinho(carrinhoCliente.ClienteId,
                    carrinhoCliente.Items.Select(i =>
                        new MensagemItemCarrinho(
                            i.Id, 
                            i.ProdutoId, 
                            i.ProdutoNome, 
                            i.PrecoUnitario, 
                            i.Quantidade, 
                            i.UrlImagem)).ToList()
                );

            // Assim que fazemos o checkout, envia um evento de integração para
            // API Pedidos para converter o carrinho em pedido e continuar com
            // processo de criação de pedido
            await _endpoint.Publish(eventMessage);

            var carrinho = await _repository.GetCarrinhoAsync(carrinhoCliente.ClienteId);

            if (carrinho == null)
            {
                return BadRequest();
            }

            return Accepted();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repository.DeleteCarrinhoAsync(id);
        }
    }
}
