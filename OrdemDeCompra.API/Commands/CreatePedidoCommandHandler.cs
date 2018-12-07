using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using MediatR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Commands
{
    public class CreatePedidoCommandHandler
        : IRequestHandler<IdentifiedCommand<CreatePedidoCommand, bool>, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<CreatePedidoCommandHandler> _logger;
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;
        private readonly HubConnection _connection;

        public CreatePedidoCommandHandler(
            ILogger<CreatePedidoCommandHandler> logger
            , IPedidoRepository pedidoRepository
            , IBus bus
            , IConfiguration configuration
            )
        {
            this._pedidoRepository = pedidoRepository;
            this._logger = logger;
            this._bus = bus;
            this._configuration = configuration;

            this._connection = new HubConnectionBuilder()
                .WithUrl($"{_configuration["SignalRServerUrl"]}usernotificationhub")
                .Build();
            this._connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this._connection.StartAsync();
            };

            this._connection.StartAsync().GetAwaiter();
        }

        public async Task<bool> Handle(IdentifiedCommand<CreatePedidoCommand, bool> request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException("Request cannot be empty");

            var cmd = request.Command;
            var guid = request.Id;

            if (cmd == null)
                throw new ArgumentNullException("Command cannot be empty");

            if (guid == Guid.Empty)
                throw new ArgumentException("Guid cannot be empty");

            var itens = cmd.Itens.Select(
                    i => new ItemPedido(i.ProdutoCodigo, i.ProdutoNome, i.ProdutoQuantidade, i.ProdutoPrecoUnitario)
                ).ToList();

            if (itens.Count == 0)
            {
                throw new NoItemsException();
            }


            foreach (var item in itens)
            {
                if (
                    string.IsNullOrWhiteSpace(item.ProdutoCodigo)
                    || string.IsNullOrWhiteSpace(item.ProdutoNome)
                    || item.ProdutoQuantidade <= 0
                    || item.ProdutoPrecoUnitario <= 0
                    )
                {
                    throw new InvalidItemException();
                }
            }


            if (string.IsNullOrWhiteSpace(cmd.ClienteId)
                 || string.IsNullOrWhiteSpace(cmd.ClienteNome)
                 || string.IsNullOrWhiteSpace(cmd.ClienteEmail)
                 || string.IsNullOrWhiteSpace(cmd.ClienteTelefone)
                 || string.IsNullOrWhiteSpace(cmd.ClienteEndereco)
                 || string.IsNullOrWhiteSpace(cmd.ClienteComplemento)
                 || string.IsNullOrWhiteSpace(cmd.ClienteBairro)
                 || string.IsNullOrWhiteSpace(cmd.ClienteMunicipio)
                 || string.IsNullOrWhiteSpace(cmd.ClienteUF)
                 || string.IsNullOrWhiteSpace(cmd.ClienteCEP)
                )
                throw new InvalidUserDataException();

            var pedido = new Pedido(itens, cmd.ClienteId,
                cmd.ClienteNome, cmd.ClienteEmail, cmd.ClienteTelefone, 
                cmd.ClienteEndereco, cmd.ClienteComplemento, cmd.ClienteBairro, 
                cmd.ClienteMunicipio, cmd.ClienteUF, cmd.ClienteCEP);
            pedido.DateCreated = DateTime.Now;

            try
            {
                Pedido novoPedido = await this._pedidoRepository.CreateOrUpdate(pedido);

                await this._connection.InvokeAsync("SendUserNotification",
                    $"{novoPedido.ClienteId}", $"Novo pedido gerado com sucesso: {novoPedido.Id}");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
