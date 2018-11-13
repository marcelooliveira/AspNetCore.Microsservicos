using Carrinho.API.Controllers;
using Carrinho.API.Model;
using CasaDoCodigo.Mensagens.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using ICarrinhoIdentityService = Carrinho.API.Services.IIdentityService;

namespace Carrinho.API.Tests
{
    public class CarrinhoControllerTest
    {
        private readonly Mock<ICarrinhoRepository> _carrinhoRepositoryMock;
        private readonly Mock<ICarrinhoIdentityService> _identityServiceMock;
        private readonly Mock<IBus> _serviceBusMock;

        public CarrinhoControllerTest()
        {
            _carrinhoRepositoryMock = new Mock<ICarrinhoRepository>();
            _identityServiceMock = new Mock<ICarrinhoIdentityService>();
            _serviceBusMock = new Mock<IBus>();
        }

        #region Get

        [Fact]
        public async Task Get_carrinho_cliente_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var carrinhoFake = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(carrinhoFake));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);

            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await carrinhoController.Get(fakeClienteId) as OkObjectResult;

            //Assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            CarrinhoCliente carrinhoCliente = Assert.IsAssignableFrom<CarrinhoCliente>(okObjectResult.Value);
            Assert.Equal(fakeClienteId, carrinhoCliente.ClienteId);
            Assert.Equal(carrinhoFake.Itens[0].ProdutoId, carrinhoCliente.Itens[0].ProdutoId);
            Assert.Equal(carrinhoFake.Itens[1].ProdutoId, carrinhoCliente.Itens[1].ProdutoId);
            Assert.Equal(carrinhoFake.Itens[2].ProdutoId, carrinhoCliente.Itens[2].ProdutoId);
        }

        [Fact]
        public async Task Get_carrinho_cliente_no_client()
        {
            //arrange
            var controller =
                new CarrinhoController(_carrinhoRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object);

            //act
            IActionResult actionResult = await controller.Get(null);

            //assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.IsType<SerializableError>(badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Get_carrinho_cliente_carrinho_not_found()
        {
            //arrange
            string clienteId = "123";
            CarrinhoCliente carrinhoFake = GetCarrinhoClienteFake(clienteId);
            _carrinhoRepositoryMock
                .Setup(r => r.GetCarrinhoAsync(clienteId))
                .ReturnsAsync((CarrinhoCliente)null);

            var controller =
                new CarrinhoController(_carrinhoRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object);

            //act
            IActionResult actionResult = await controller.Get(clienteId);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            CarrinhoCliente carrinhoCliente = Assert.IsAssignableFrom<CarrinhoCliente>(okObjectResult.Value);
            Assert.Equal(clienteId, carrinhoCliente.ClienteId);
        }
        #endregion

        #region Post
        [Fact]
        public async Task Post_carrinho_cliente_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.UpdateCarrinhoAsync(It.IsAny<CarrinhoCliente>()))
                .Returns(Task.FromResult(fakeCarrinhoCliente));
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await carrinhoController.Post(fakeCarrinhoCliente) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CarrinhoCliente)actionResult.Value).ClienteId, fakeClienteId);
        }

        [Fact]
        public async Task Post_carrinho_cliente_not_found()
        {
            //Arrange
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.UpdateCarrinhoAsync(It.IsAny<CarrinhoCliente>()))
                .ThrowsAsync(new KeyNotFoundException())
                .Verifiable();
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);

            var actionResult = await carrinhoController.Post(fakeCarrinhoCliente);

            //Assert
            NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task Post_carrinho_cliente_invalid_model()
        {
            //Arrange
            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            controller.ModelState.AddModelError("ClienteId", "Required");

            //Act
            var actionResult = await controller.Post(new CarrinhoCliente());

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        }
        
        #endregion

        #region Checkout
        [Fact]
        public async Task Fazer_Checkout_Sem_Cesta_Deve_Retornar_BadRequest()
        {
            //Arrange
            var fakeClienteId = "2";
            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                .ReturnsAsync((CarrinhoCliente)null);
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object);
            CadastroViewModel input = new CadastroViewModel();
            carrinhoController.ModelState.AddModelError("Email", "Required");

            //Act
            ActionResult<bool> actionResult = await carrinhoController.Checkout(fakeClienteId, input);

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            CadastroViewModel cadastroViewModel = Assert.IsAssignableFrom<CadastroViewModel>(badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Fazer_Checkout_Com_Carrinho_Deveria_Publicar_CheckoutEvent()
        {
            //arrange
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                 .Returns(Task.FromResult(fakeCarrinhoCliente));

            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);

            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object);

            carrinhoController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[] { new Claim("unique_name", "testuser") }))
                }
            };

            //Act
            ActionResult<bool> actionResult = await carrinhoController.Checkout(fakeClienteId, new CadastroViewModel());

            //assert
            _serviceBusMock.Verify(mock => mock.Publish(It.IsAny<CheckoutEvent>(), null), Times.Once);
            Assert.NotNull(actionResult);
        }

        #endregion

        private CarrinhoCliente GetCarrinhoClienteFake(string fakeClienteId)
        {
            return new CarrinhoCliente(fakeClienteId)
            {
                ClienteId = fakeClienteId,
                Itens = new List<ItemCarrinho>()
                {
                    new ItemCarrinho("001", "001", "produto 001", 12.34m, 1),
                    new ItemCarrinho("002", "002", "produto 002", 23.45m, 2),
                    new ItemCarrinho("003", "003", "produto 003", 34.56m, 3)
                }
            };
        }

    }
}
