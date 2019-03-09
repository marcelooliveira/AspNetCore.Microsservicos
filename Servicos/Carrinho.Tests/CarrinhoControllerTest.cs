using Carrinho.API.Controllers;
using Carrinho.API.Model;
using CasaDoCodigo.Mensagens.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Rebus.Bus;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Mock<ILogger<CarrinhoController>> _loggerMock;        
        private readonly Mock<IConfiguration> _configurationMock;

        public CarrinhoControllerTest()
        {
            _carrinhoRepositoryMock = new Mock<ICarrinhoRepository>();
            _identityServiceMock = new Mock<ICarrinhoIdentityService>();
            _serviceBusMock = new Mock<IBus>();
            _loggerMock = new Mock<ILogger<CarrinhoController>>();
            _loggerMock = new Mock<ILogger<CarrinhoController>>();
            _configurationMock = new Mock<IConfiguration>();
        }

        #region Get

        [Fact]
        public async Task Get_carrinho_cliente_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var carrinhoFake = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock
                .Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(carrinhoFake))
                .Verifiable();
            _identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId);
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null));

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            var actionResult = await carrinhoController.Get(fakeClienteId) as OkObjectResult;

            //Assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            CarrinhoCliente carrinhoCliente = Assert.IsAssignableFrom<CarrinhoCliente>(okObjectResult.Value);
            Assert.Equal(fakeClienteId, carrinhoCliente.ClienteId);
            Assert.Equal(carrinhoFake.Itens[0].ProdutoId, carrinhoCliente.Itens[0].ProdutoId);
            Assert.Equal(carrinhoFake.Itens[1].ProdutoId, carrinhoCliente.Itens[1].ProdutoId);
            Assert.Equal(carrinhoFake.Itens[2].ProdutoId, carrinhoCliente.Itens[2].ProdutoId);
            _carrinhoRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task Get_carrinho_cliente_no_client()
        {
            //arrange
            var controller =
                new CarrinhoController(_carrinhoRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object,
                _loggerMock.Object, _configurationMock.Object);

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
                .ReturnsAsync((CarrinhoCliente)null)
                .Verifiable();

            var controller =
                new CarrinhoController(_carrinhoRepositoryMock.Object,
                _identityServiceMock.Object, _serviceBusMock.Object,
                _loggerMock.Object, _configurationMock.Object);

            //act
            IActionResult actionResult = await controller.Get(clienteId);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            CarrinhoCliente carrinhoCliente = Assert.IsAssignableFrom<CarrinhoCliente>(okObjectResult.Value);
            Assert.Equal(clienteId, carrinhoCliente.ClienteId);
            _carrinhoRepositoryMock.Verify();
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
                .Returns(Task.FromResult(fakeCarrinhoCliente))
                .Verifiable();
            _serviceBusMock.Setup(x => x.Publish(It.IsAny<CheckoutEvent>(), null))
                .Verifiable();

            //Act
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            var actionResult = await carrinhoController.Post(fakeCarrinhoCliente) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(((CarrinhoCliente)actionResult.Value).ClienteId, fakeClienteId);

            _carrinhoRepositoryMock.Verify();
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
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            var actionResult = await carrinhoController.Post(fakeCarrinhoCliente);

            //Assert
            NotFoundResult notFoundResult = Assert.IsType<NotFoundResult>(actionResult);
            _carrinhoRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task Post_carrinho_cliente_invalid_model()
        {
            //Arrange
            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);
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

            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            CadastroViewModel input = new CadastroViewModel();
            carrinhoController.ModelState.AddModelError("Email", "Required");

            //Act
            ActionResult<bool> actionResult = await carrinhoController.Checkout(fakeClienteId, input);

            //Assert
            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<SerializableError>(badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Fazer_Checkout_Carrinho_Not_Found()
        {
            //Arrange
            var fakeClienteId = "123";
            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                .ThrowsAsync(new KeyNotFoundException());
            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            CadastroViewModel input = new CadastroViewModel();

            //Act
            ActionResult<bool> actionResult = await carrinhoController.Checkout(fakeClienteId, input);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }


        [Fact]
        public async Task Fazer_Checkout_Com_Carrinho_Deveria_Publicar_CheckoutEvent()
        {
            //arrange
            var fakeClienteId = "1";
            var fakeCarrinhoCliente = GetCarrinhoClienteFake(fakeClienteId);

            _carrinhoRepositoryMock.Setup(x => x.GetCarrinhoAsync(It.IsAny<string>()))
                 .Returns(Task.FromResult(fakeCarrinhoCliente))
                .Verifiable();

            //_identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(fakeClienteId)
            //    .Verifiable();

            var carrinhoController = new CarrinhoController(
                _carrinhoRepositoryMock.Object, _identityServiceMock.Object, _serviceBusMock.Object,
                _loggerMock.Object, _configurationMock.Object);

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
            _carrinhoRepositoryMock.Verify();
            //_identityServiceMock.Verify();
        }

        #endregion

        #region AddItem
        [Fact]
        public async Task AddItem_success()
        {
            //arrange
            var clienteId = "123";
            var carrinho = GetCarrinhoClienteFake(clienteId);
            ItemCarrinho input = new ItemCarrinho("004", "004", "produto 004", 45.67m, 4);
            var itens = carrinho.Itens;
            itens.Add(input);
            _carrinhoRepositoryMock
                .Setup(c => c.AddCarrinhoAsync(clienteId, It.IsAny<ItemCarrinho>()))
                .ReturnsAsync(new CarrinhoCliente
                {
                    ClienteId = clienteId,
                    Itens = itens
                })
                .Verifiable();

            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            //act
            ActionResult<CarrinhoCliente> actionResult = await controller.AddItem(clienteId, input);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            CarrinhoCliente carrinhoCliente = Assert.IsAssignableFrom<CarrinhoCliente>(okObjectResult.Value);
            Assert.Equal(4, carrinhoCliente.Itens.Count());
            _carrinhoRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task AddItem_carrinho_notfound()
        {
            //arrange
            var clienteId = "123";
            ItemCarrinho input = new ItemCarrinho("004", "004", "produto 004", 45.67m, 4);
            _carrinhoRepositoryMock
                .Setup(c => c.AddCarrinhoAsync(clienteId, It.IsAny<ItemCarrinho>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            //act
            ActionResult<CarrinhoCliente> actionResult = await controller.AddItem(clienteId, input);

            //assert
            NotFoundObjectResult notFoundObjectResult =  Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(clienteId, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task AddItem_carrinho_invalid_model()
        {
            //arrange
            var clienteId = "123";
            ItemCarrinho input = new ItemCarrinho("004", "004", "produto 004", 45.67m, 4);
            _carrinhoRepositoryMock
                .Setup(c => c.AddCarrinhoAsync(clienteId, It.IsAny<ItemCarrinho>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            controller.ModelState.AddModelError("ProdutoId", "Required");
            
            //act
            ActionResult<CarrinhoCliente> actionResult = await controller.AddItem(clienteId, input);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        #endregion

        #region UpdateItem
        [Fact]
        public async Task UpdateItem_success()
        {
            //arrange
            var clienteId = "123";
            var carrinho = GetCarrinhoClienteFake(clienteId);
            ItemCarrinho input = new ItemCarrinho("004", "004", "produto 004", 45.67m, 4);
            var itens = carrinho.Itens;
            itens.Add(input);
            _carrinhoRepositoryMock
                .Setup(c => c.UpdateCarrinhoAsync(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ReturnsAsync(new UpdateQuantidadeOutput(input,
                new CarrinhoCliente
                {
                    ClienteId = clienteId,
                    Itens = itens
                }))
                .Verifiable();

            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            //act
            ActionResult<UpdateQuantidadeOutput> actionResult = await controller.UpdateItem(clienteId, new UpdateQuantidadeInput(input.ProdutoId, input.Quantidade));

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            UpdateQuantidadeOutput updateQuantidadeOutput = Assert.IsAssignableFrom<UpdateQuantidadeOutput>(okObjectResult.Value);
            Assert.Equal(input.ProdutoId, updateQuantidadeOutput.ItemPedido.ProdutoId);
            _carrinhoRepositoryMock.Verify();
            _identityServiceMock.Verify();
            _serviceBusMock.Verify();
        }

        [Fact]
        public async Task UpdateItem_carrinho_notfound()
        {
            //arrange
            var clienteId = "123";
            ItemCarrinho input = new ItemCarrinho("004", "004", "produto 004", 45.67m, 4);
            _carrinhoRepositoryMock
                .Setup(c => c.UpdateCarrinhoAsync(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ThrowsAsync(new KeyNotFoundException());
            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            //act
            ActionResult<UpdateQuantidadeOutput> actionResult = await controller.UpdateItem(clienteId, new UpdateQuantidadeInput(input.ProdutoId, input.Quantidade));

            //assert
            NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(clienteId, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task UpdateItem_carrinho_invalid_model()
        {
            //arrange
            var clienteId = "123";
            ItemCarrinho input = new ItemCarrinho("004", "004", "produto 004", 45.67m, 4);
            var controller = new CarrinhoController(
                _carrinhoRepositoryMock.Object,
                _identityServiceMock.Object,
                _serviceBusMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);

            controller.ModelState.AddModelError("ProdutoId", "Required");

            //act
            ActionResult<UpdateQuantidadeOutput> actionResult = await controller.UpdateItem(clienteId, new UpdateQuantidadeInput(input.ProdutoId, input.Quantidade));

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
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
