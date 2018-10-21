using CasaDoCodigo.Carrinho.Model;
using CasaDoCodigo.Mensagens.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CasaDoCodigo.Carrinho.Controllers
{
    /// <summary>
    /// Fornece funcionalidades do carrinho de compras da Casa do Código
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoRepository _repository;
        private readonly IBus _bus;
        private readonly ILoggerFactory _loggerFactory;

        public CarrinhoController(ICarrinhoRepository repository
            , IBus bus
            , ILoggerFactory loggerFactory
            )
        {
            _repository = repository;
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
                return Ok(new CarrinhoCliente() { ClienteId = id });
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
        [Route("[action]/{clienteId}")]
        [ProducesResponseType(typeof(ItemCarrinho), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateItem(string clienteId, [FromBody] ItemCarrinho input)
        {
            var carrinho = await _repository.UpdateCarrinhoAsync(clienteId, input);
            return Ok(carrinho);
        }

        /// <summary>
        /// Fecha o carrinho de compras e finaliza o pedido
        /// </summary>
        /// <param name="carrinhoCliente">Dados do carrinho de compras</param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
         public async Task<IActionResult> Checkout([FromBody]CarrinhoCliente carrinhoCliente, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var eventMessage 
                = new CheckoutEvent(carrinhoCliente.ClienteId,
                    carrinhoCliente.Itens.Select(i =>
                        new CheckoutItem(
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
            await _bus.Publish(eventMessage);

            var carrinho = await _repository.GetCarrinhoAsync(carrinhoCliente.ClienteId);

            if (carrinho == null)
            {
                return BadRequest();
            }

            return Accepted();
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
    }
}
