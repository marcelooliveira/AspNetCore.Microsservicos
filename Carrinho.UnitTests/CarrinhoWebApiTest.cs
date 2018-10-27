using Carrinho.API.Controllers;
using Carrinho.API.Model;
using CasaDoCodigo.Mensagens.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using ICarrinhoIdentityService = Carrinho.API.Services.IIdentityService;

namespace Carrinho.UnitTests
{
    public class CarrinhoWebApiTest
    {
        private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
        private readonly Mock<ICarrinhoIdentityService> _identityServiceMock;
        private readonly Mock<IBus> _serviceBusMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;

        public CarrinhoWebApiTest()
        {
            _carrinhoRepositoryMock = new Mock<ICarrinhoRepository>();
            _identityServiceMock = new Mock<ICarrinhoIdentityService>();
            _serviceBusMock = new Mock<IBus>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
        }

        [Fact]
        public async Task Get_carrinho_cliente_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(fakeCarrinhoCliente));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);

            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutAceitoEvent>(), null));

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object, 
                _identityServiceMock.Object, 
                _serviceBusMock.Object,
                _loggerFactoryMock.Object);

            var actionResult = await carrinhoController.Get(fakeClienteId) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CarrinhoCliente)actionResult.Value).ClienteId, fakeClienteId);
        }

        [Fact]
        public async Task Post_carrinho_cliente_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.UpdateCarrinhoAsync(It.IsAny<CarrinhoCliente>()))
                .Returns(Task.FromResult(fakeCarrinhoCliente));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutAceitoEvent>(), null));

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerFactoryMock.Object);

            var actionResult = await carrinhoController.Post(fakeCarrinhoCliente) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CarrinhoCliente)actionResult.Value).ClienteId, fakeClienteId);
        }

        [Fact]
        public async Task Fazer_Checkout_Sem_Cesta_Deve_Retornar_BadRequest()
        {
            //Arrange

            var fakeClienteId = "2";
            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult((CarrinhoCliente)null));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);

            //Act

            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerFactoryMock.Object);

            var result = await carrinhoController.Checkout(new CarrinhoCheckout(), Guid.NewGuid().ToString()) as BadRequestResult;

            //Assert

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Fazer_Checkout_Com_Carrinho_Deveria_Publicar_CheckoutAceitoEvent()
        {
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                 .Returns(Task.FromResult(fakeCarrinhoCliente));

            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);

            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object, _loggerFactoryMock.Object);

            carrinhoController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[] { new Claim("unique_name", "testuser") }))
                }
            };

            //Act
            var result = await carrinhoController.Checkout(new CarrinhoCheckout(), Guid.NewGuid().ToString()) as AcceptedResult;

            _serviceBusMock.Verify(mock => mock.Publish(It.IsAny<CheckoutAceitoEvent>(), null), Times.Once);

            Assert.NotNull(result);
        }

        private CarrinhoCliente GetCarrinhoClienteFake(string fakeClienteId)
        {
            return new CarrinhoCliente(fakeClienteId)
            {
                Itens = new List<ItemCarrinho>()
                {
                    new ItemCarrinho()
                }
            };
        }

    }
}
