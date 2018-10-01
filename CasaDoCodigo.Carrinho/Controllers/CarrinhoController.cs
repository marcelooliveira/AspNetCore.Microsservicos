using CasaDoCodigo.Carrinho.Model;
using CasaDoCodigo.Mensagens.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CasaDoCodigo.Carrinho.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
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

        [HttpPost("{clienteId}", Name = "PostItem")]
        [ProducesResponseType(typeof(ItemCarrinho), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post(string clienteId, [FromBody] ItemCarrinho input)
        {
            var carrinho = await _repository.UpdateCarrinhoAsync(clienteId, input);
            return Ok(carrinho);
        }

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
            //await _endpoint.Publish(eventMessage);

            await _bus.Publish(eventMessage);

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
