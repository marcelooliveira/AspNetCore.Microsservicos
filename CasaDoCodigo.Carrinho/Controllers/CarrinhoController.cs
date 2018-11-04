using Carrinho.API.Model;
using Carrinho.API.Services;
using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Carrinho.API.Controllers
{
    /// <summary>
    /// Fornece funcionalidades do carrinho de compras da Casa do Código
    /// </summary>
    [Route("api/[controller]")]
    //[Authorize]
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IBus _bus;
        private readonly ILoggerFactory _loggerFactory;
        
        public CarrinhoController(ICarrinhoRepository repository
            , IIdentityService identityService
            , IBus bus
            , ILoggerFactory loggerFactory
            )
        {
            _repository = repository;
            _identityService = identityService;
            //_endpoint = endpoint;
            _bus = bus;
            _loggerFactory = loggerFactory;
            _loggerFactory.AddDebug();
        }

        //GET /id
        /// <summary>
        /// Obtém o carrinho de compras
        /// </summary>
        /// <param name="id">Id do cliente do carrinho</param>
        /// <returns>O carrinho de compras</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CarrinhoCliente), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var carrinho = await _repository.GetCarrinhoAsync(id);
            if (carrinho == null)
            {
                return Ok(new CarrinhoCliente(id));
            }
            return Ok(carrinho);
        }

        //POST /value
        /// <summary>
        /// Salva o carrinho de compras do cliente
        /// </summary>
        /// <param name="input">Dados do carrinho de compras</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CarrinhoCliente), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] CarrinhoCliente input)
        {
            var carrinho = await _repository.UpdateCarrinhoAsync(input);
            return Ok(carrinho);
        }

        /// <summary>
        /// Adiciona um item no carrinho de compras do cliente
        /// </summary>
        /// <param name="clienteId">Id do cliente</param>
        /// <param name="input">Novo item a inserir no carrinho de compras</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]/{clienteId}")]
        [ProducesResponseType(typeof(ItemCarrinho), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddItem(string clienteId, [FromBody] ItemCarrinho input)
        {
            var carrinho = await _repository.AddCarrinhoAsync(clienteId, input);
            return Ok(carrinho);
        }

        /// <summary>
        /// Atualiza a quantidade do item do carrinho de compras
        /// </summary>
        /// <param name="clienteId">Id do cliente</param>
        /// <param name="input">Item do carrinho de compras cuja quantidade será atualizada</param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]/{clienteId}")]
        [ProducesResponseType(typeof(ItemCarrinho), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateItem(string clienteId, [FromBody] ItemCarrinho input)
        {
            var carrinho = await _repository.UpdateCarrinhoAsync(clienteId, input);
            return Ok(carrinho);
        }

        /// <summary>
        /// Remove um item do carrinho de compras
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repository.DeleteCarrinhoAsync(id);
        }

        [Route("[action]/{clienteId}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout(string clienteId, [FromBody] CadastroViewModel input)
        {
            var carrinho = await _repository.GetCarrinhoAsync(clienteId);

            if (carrinho == null)
            {
                return BadRequest();
            }

            var itens = carrinho.Itens.Select(i =>
                    new CheckoutEventItem(i.Id, i.ProdutoId, i.ProdutoNome, i.PrecoUnitario, i.Quantidade)).ToList();

            var checkoutEvent
                = new CheckoutEvent
                 (clienteId, input.Nome, input.Email, input.Telefone
                    , input.Endereco, input.Complemento, input.Bairro
                    , input.Municipio, input.UF, input.CEP
                    , Guid.NewGuid()
                    , itens);

            // Assim que fazemos a finalização, envia um evento de integração para
            // API OrdemDeCompra converter o carrinho em pedido e continuar com
            // processo de criação de pedido
            await _bus.Publish(checkoutEvent);

            var cadastroEvent
                = new CadastroEvent
                 (clienteId, input.Nome, input.Email, input.Telefone
                    , input.Endereco, input.Complemento, input.Bairro
                    , input.Municipio, input.UF, input.CEP);

            await _bus.Publish(cadastroEvent);

            await _repository.DeleteCarrinhoAsync(clienteId);

            return Accepted();
        }
    }
}
