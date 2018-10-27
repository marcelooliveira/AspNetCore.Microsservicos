using Carrinho.API.Controllers;
using Carrinho.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Rebus.Bus;
using System;
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

        //[Fact]
        //public async Task Get_customer_basket_success()
        //{
        //    //Arrange
        //    var fakeCustomerId = "1";
        //    var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeCustomerId);

        //    _carrinhoRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
        //        .Returns(Task.FromResult(fakeCarrinhoCliente));
        //    _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        //    _serviceBusMock.Setup(x => x.Publish(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()));

        //    //Act
        //    var carrinhoController = new CarrinhoController(
        //        _carrinhoRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);
        //    var actionResult = await carrinhoController.Get(fakeCustomerId) as OkObjectResult;

        //    //Assert
        //    Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        //    Assert.Equal(((CarrinhoCliente)actionResult.Value).BuyerId, fakeCustomerId);
        //}

        //[Fact]
        //public async Task Post_customer_basket_success()
        //{
        //    //Arrange
        //    var fakeCustomerId = "1";
        //    var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeCustomerId);

        //    _carrinhoRepositoryMock.Setup(x => x.UpdateBasketAsync(It.IsAny<CarrinhoCliente>()))
        //        .Returns(Task.FromResult(fakeCarrinhoCliente));
        //    _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);
        //    _serviceBusMock.Setup(x => x.Publish(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()));

        //    //Act
        //    var carrinhoController = new CarrinhoController(
        //        _carrinhoRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

        //    var actionResult = await carrinhoController.Post(fakeCarrinhoCliente) as OkObjectResult;

        //    //Assert
        //    Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        //    Assert.Equal(((CarrinhoCliente)actionResult.Value).BuyerId, fakeCustomerId);
        //}

        [Fact]
        public async Task Fazer_Checkout_Sem_Cesta_Deve_Retornar_Bad_Request()
        {
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
            Assert.NotNull(result);
        }

        //[Fact]
        //public async Task Doing_Checkout_Wit_Basket_Should_Publish_UserCheckoutAccepted_Integration_Event()
        //{
        //    var fakeCustomerId = "1";
        //    var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeCustomerId);

        //    _carrinhoRepositoryMock.Setup(x => x.GetBasketAsync(It.IsAny<string>()))
        //         .Returns(Task.FromResult(fakeCarrinhoCliente));

        //    _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeCustomerId);

        //    var carrinhoController = new CarrinhoController(
        //        _carrinhoRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

        //    carrinhoController.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = new ClaimsPrincipal(
        //                new ClaimsIdentity(new Claim[] { new Claim("unique_name", "testuser") }))
        //        }
        //    };

        //    //Act
        //    var result = await carrinhoController.Checkout(new CarrinhoCheckout(), Guid.NewGuid().ToString()) as AcceptedResult;

        //    _serviceBusMock.Verify(mock => mock.Publish(It.IsAny<UserCheckoutAcceptedIntegrationEvent>()), Times.Once);

        //    Assert.NotNull(result);
        //}

        //private CarrinhoCliente GetCarrinhoClienteFake(string fakeCustomerId)
        //{
        //    return new CarrinhoCliente(fakeCustomerId)
        //    {
        //        Items = new List<BasketItem>()
        //        {
        //            new BasketItem()
        //        }
        //    };
        //}

    }
}
