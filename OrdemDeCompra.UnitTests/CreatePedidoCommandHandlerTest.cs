using CasaDoCodigo.Mensagens.Commands;
using CasaDoCodigo.OrdemDeCompra.Commands;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OrdemDeCompra.UnitTests
{
    public class CreatePedidoCommandHandlerTest
    {
        private readonly Mock<ILogger<CreatePedidoCommandHandler>> loggerMock;
        private readonly Mock<IPedidoRepository> pedidoRepositoryMock;
        private readonly Mock<IBus> busMock;

        public CreatePedidoCommandHandlerTest()
        {
            this.loggerMock = new Mock<ILogger<CreatePedidoCommandHandler>>();
            this.pedidoRepositoryMock = new Mock<IPedidoRepository>();
            this.busMock = new Mock<IBus>();
        }

        [Fact]
        public async Task Handle_request_is_null()
        {
            //arrange
            CancellationToken token = default(System.Threading.CancellationToken);
            IdentifiedCommand<CreatePedidoCommand, bool> request = null;
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_command_is_null()
        {
            //arrange
            CancellationToken token = default(System.Threading.CancellationToken);
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(null, new Guid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_guid_is_empty()
        {
            //arrange
            CancellationToken token = default(System.Threading.CancellationToken);
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(new CreatePedidoCommand(), Guid.Empty);
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_items_is_empty()
        {
            //arrange
            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand();
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<NoItemsException>(async () => await handler.Handle(request, token));
        }

        [Theory]
        [InlineData("", "produto 001", 1, 12.34)]
        [InlineData("001", "", 1, 12.34)]
        [InlineData("001", "produto 001", 0, 12.34)]
        [InlineData("001", "produto 001", -1, 12.34)]
        [InlineData("001", "produto 001", 1, -10)]
        public async Task Handle_invalid_item(string produtoCodigo, string produtoNome, int produtoQuantidade, decimal produtoPrecoUnitario)
        {
            //arrange
            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand(new List<CreatePedidoCommandItem>
            {
                new CreatePedidoCommandItem(produtoCodigo, produtoNome, produtoQuantidade, produtoPrecoUnitario)
            }
            , "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<InvalidItemException>(async () => await handler.Handle(request, token));
        }

        [Theory]
        [InlineData("", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "")]
        public async Task Handle_invalid_user_data(string clienteId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            //arrange
            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand(new List<CreatePedidoCommandItem>
            {
                new CreatePedidoCommandItem("001", "produto 001", 1, 12.34m)
            }
            , clienteId, clienteNome, clienteEmail, clienteTelefone, clienteEndereco, clienteComplemento, clienteBairro, clienteMunicipio, clienteUF, clienteCEP);
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<InvalidUserDataException>(async () => await handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_success()
        {
            //arrange
            var pedido = new Pedido(
                new List<ItemPedido> {
                    new ItemPedido("001", "produto 001", 1, 12.34m),
                    new ItemPedido("002", "produto 002", 2, 23.45m)
                },
                "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");

            CancellationToken token = default(CancellationToken);
            CreatePedidoCommand command = new CreatePedidoCommand(new List<CreatePedidoCommandItem>
            {
                new CreatePedidoCommandItem("001", "produto 001", 1, 12.34m),
                new CreatePedidoCommandItem("002", "produto 002", 2, 23.45m)
            }
            , "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            IdentifiedCommand<CreatePedidoCommand, bool> request = new IdentifiedCommand<CreatePedidoCommand, bool>(command, Guid.NewGuid());
            pedidoRepositoryMock
                .Setup(r => r.CreateOrUpdate(It.IsAny<Pedido>()))
                .ReturnsAsync(pedido)
                .Verifiable();

            var handler = new CreatePedidoCommandHandler(loggerMock.Object, pedidoRepositoryMock.Object, busMock.Object);

            //act
            bool result = await handler.Handle(request, token);

            //assert
            Assert.True(result);

            pedidoRepositoryMock.Verify();
        }
    }
}
