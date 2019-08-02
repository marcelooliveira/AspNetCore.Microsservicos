using Carrinho.API.Model;
using Carrinho.API.Services;
using CasaDoCodigo.Mensagens.Events;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Carrinho.API.Controllers
{
    /// <summary>
    /// Fornece funcionalidades do carrinho de compras da Casa do Código
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class CarrinhoController : Controller
    {
        private EventId EventId_Checkout = new EventId(1001, "Checkout");
        private EventId EventId_Registry = new EventId(1002, "Cadastro");
        private readonly ICarrinhoRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly IBus _bus;
        private readonly ILogger<CarrinhoController> _logger;
        private readonly IConfiguration _configuration;
        private HubConnection _connection;

        public CarrinhoController(ICarrinhoRepository repository
            , IIdentityService identityService
            , IBus bus
            , ILogger<CarrinhoController> logger
            , IConfiguration configuration)
        {
            _repository = repository;
            _identityService = identityService;
            _bus = bus;
            _logger = logger;
            _configuration = configuration;

            SetupSignalRConnection();
        }

        //GET /id
        /// <summary>
        /// Obtém o carrinho de compras
        /// </summary>
        /// <param name="id">Id do cliente do carrinho</param>
        /// <returns>O carrinho de compras</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CarrinhoCliente), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(ModelState);
            }

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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Post([FromBody] CarrinhoCliente input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(input);
            }

            try
            {
                var carrinho = await _repository.UpdateCarrinhoAsync(input);

                await this._connection
                    .InvokeAsync("UpdateUserBasketCount", $"{input.ClienteId}", carrinho.Itens.Count);

                return Ok(carrinho);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
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
        public async Task<ActionResult<CarrinhoCliente>> AddItem(string clienteId, [FromBody] ItemCarrinho input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var carrinho = await _repository.AddCarrinhoAsync(clienteId, input);

                await this._connection
                    .InvokeAsync("UpdateUserBasketCount", $"{clienteId}", carrinho.Itens.Count);

                return Ok(carrinho);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(clienteId);
            }
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
        public async Task<ActionResult<UpdateQuantidadeOutput>> UpdateItem(string clienteId, [FromBody] UpdateQuantidadeInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var output = await _repository.UpdateCarrinhoAsync(clienteId, input);

                await this._connection
                    .InvokeAsync("UpdateUserBasketCount", $"{clienteId}", output.CarrinhoCliente.Itens.Count);

                return Ok(output);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(clienteId);
            }

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
        public async Task<ActionResult<bool>> Checkout(string clienteId, [FromBody] CadastroViewModel cadastroViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CarrinhoCliente carrinho;
            try
            {
                carrinho = await _repository.GetCarrinhoAsync(clienteId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            await PublicarEventoFechamentoDoCarrinho(clienteId, cadastroViewModel, carrinho);

            await PublicarEventoAlteracaoDoCadastro(clienteId, cadastroViewModel);

            try
            {
                await _repository.DeleteCarrinhoAsync(clienteId);

                await this._connection
                    .InvokeAsync("UpdateUserBasketCount", $"{clienteId}", 0);

                return Accepted(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        private async Task PublicarEventoFechamentoDoCarrinho(string clienteId, CadastroViewModel input, CarrinhoCliente carrinho)
        {
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

            _logger.LogInformation(eventId: EventId_Checkout, message: "Evento de check out foi enviado: {CheckoutEvent}", args: checkoutEvent);
        }

        private void SetupSignalRConnection()
        {
            Uri baseSignalRUri = new Uri(_configuration["SignalRServerUrl"]);
            Uri userCounterDataHubUri = new Uri(baseSignalRUri, "usercounterdatahub");

            this._connection = new HubConnectionBuilder()
                .WithUrl(userCounterDataHubUri, HttpTransportType.WebSockets)
                .Build();
            this._connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this._connection.StartAsync();
            };

            this._connection.StartAsync().GetAwaiter();
        }

        private async Task PublicarEventoAlteracaoDoCadastro(string clienteId, CadastroViewModel input)
        {
            var cadastroEvent
                = new CadastroEvent
                 (clienteId, input.Nome, input.Email, input.Telefone
                    , input.Endereco, input.Complemento, input.Bairro
                    , input.Municipio, input.UF, input.CEP);

            await _bus.Publish(cadastroEvent);

            _logger.LogInformation(eventId: EventId_Registry, message: "Evento de Cadastro foi enviado: {RegistryEvent}", args: cadastroEvent);
        }
    }
}
