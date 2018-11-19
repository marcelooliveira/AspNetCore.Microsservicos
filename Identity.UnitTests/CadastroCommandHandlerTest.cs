using CasaDoCodigo.Mensagens.Commands;
using Identity.API.Commands;
using Identity.API.Managers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Identity.UnitTests
{
    public class CadastroCommandHandlerTest
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<ILogger<CadastroCommandHandler>> loggerMock;
        private readonly Mock<IClaimsManager> claimsManagerMock;

        public CadastroCommandHandlerTest()
        {
            this.mediatorMock = new Mock<IMediator>();
            this.loggerMock = new Mock<ILogger<CadastroCommandHandler>>();
            this.claimsManagerMock = new Mock<IClaimsManager>();
        }

        [Fact]
        public async Task Handle_request_is_null()
        {
            //arrange
            var handler = new CadastroCommandHandler(mediatorMock.Object, loggerMock.Object, claimsManagerMock.Object);

            IdentifiedCommand<CadastroCommand, bool> request = null;
            CancellationToken token = default(System.Threading.CancellationToken);
            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_command_is_null()
        {
            //arrange
            var handler = new CadastroCommandHandler(mediatorMock.Object, loggerMock.Object, claimsManagerMock.Object);

            IdentifiedCommand<CadastroCommand, bool> request = new IdentifiedCommand<CadastroCommand, bool>(null, Guid.NewGuid());
            CancellationToken token = default(System.Threading.CancellationToken);
            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_guid_is_empty()
        {
            //arrange
            var handler = new CadastroCommandHandler(mediatorMock.Object, loggerMock.Object, claimsManagerMock.Object);

            IdentifiedCommand<CadastroCommand, bool> request = new IdentifiedCommand<CadastroCommand, bool>(new CadastroCommand(), Guid.Empty);
            CancellationToken token = default(System.Threading.CancellationToken);
            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(request, token));
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
            var handler = new CadastroCommandHandler(mediatorMock.Object, loggerMock.Object, claimsManagerMock.Object);

            CadastroCommand command = new CadastroCommand(clienteId, clienteNome, clienteEmail, clienteTelefone, clienteEndereco, clienteComplemento, clienteBairro, clienteMunicipio, clienteUF, clienteCEP);
            IdentifiedCommand<CadastroCommand, bool> request = new IdentifiedCommand<CadastroCommand, bool>(command, Guid.NewGuid());
            CancellationToken token = default(System.Threading.CancellationToken);
            //act
            //assert
            await Assert.ThrowsAsync<InvalidUserDataException>(() => handler.Handle(request, token));
        }

        [Fact]
        public async Task Handle_success()
        {
            //arrange
            claimsManagerMock
                .Setup(c => c.AddUpdateClaim(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(Task.CompletedTask);

            var handler = new CadastroCommandHandler(mediatorMock.Object, loggerMock.Object, claimsManagerMock.Object);
            CadastroCommand command = new CadastroCommand("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");

            IdentifiedCommand<CadastroCommand, bool> request = new IdentifiedCommand<CadastroCommand, bool>(command, Guid.NewGuid());
            CancellationToken token = default(CancellationToken);
            
            //act
            var result = await handler.Handle(request, token);

            //assert
            Assert.True(result);
        }
    }
}
