using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using MVC.Model.Redis;
using MVC.Models;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CarrinhoControllerTest : BaseControllerTest
    {
        private readonly Mock<ILogger<CarrinhoController>> loggerMock;
        private readonly Mock<ICatalogoService> catalogoServiceMock;
        private readonly Mock<ICarrinhoService> carrinhoServiceMock;
        private readonly Mock<IUserRedisRepository> userRedisRepositoryMock;

        public CarrinhoControllerTest() : base()
        {
            loggerMock = new Mock<ILogger<CarrinhoController>>();
            catalogoServiceMock = new Mock<ICatalogoService>();
            carrinhoServiceMock = new Mock<ICarrinhoService>();
            userRedisRepositoryMock = new Mock<IUserRedisRepository>();
        }

        #region Index
        [Fact]
        public async Task Index_Success()
        {
            //arrange
            var clienteId = "cliente_id";
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];
            catalogoServiceMock
                .Setup(c => c.GetProduto(testProduct.Codigo))
                .ReturnsAsync(testProduct)
                .Verifiable();

            var itemCarrinho = new ItemCarrinho(testProduct.Codigo, testProduct.Codigo, testProduct.Nome, testProduct.Preco, 1, testProduct.UrlImagem);
            carrinhoServiceMock
                .Setup(c => c.AddItem(clienteId, It.IsAny<ItemCarrinho>()))
                .ReturnsAsync(
                new CarrinhoCliente(clienteId,
                    new List<ItemCarrinho>
                    {
                        itemCarrinho
                    }))
                .Verifiable();
            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index(testProduct.Codigo);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarrinhoCliente>(viewResult.Model);
            Assert.Equal(model.Itens[0].ProdutoNome, produtos[0].Nome);
            catalogoServiceMock.Verify();
        }

        [Fact]
        public async Task Index_Without_Product_Success()
        {
            //arrange
            var clienteId = "cliente_id";
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];

            var itemCarrinho = new ItemCarrinho(testProduct.Codigo, testProduct.Codigo, testProduct.Nome, testProduct.Preco, 1, testProduct.UrlImagem);
            carrinhoServiceMock
                .Setup(c => c.GetCarrinho(clienteId))
                .ReturnsAsync(
                new CarrinhoCliente(clienteId,
                    new List<ItemCarrinho>
                    {
                        itemCarrinho
                    }))
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarrinhoCliente>(viewResult.Model);
            Assert.Equal(model.Itens[0].ProdutoNome, produtos[0].Nome);
            carrinhoServiceMock.Verify();
        }

        [Fact]
        public async Task Index_BrokenCircuitException()
        {
            //arrange
            var clienteId = "cliente_id";
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];
            catalogoServiceMock
                .Setup(c => c.GetProduto(It.IsAny<string>()))
                .ThrowsAsync(new BrokenCircuitException())
                .Verifiable();

            var itemCarrinho = new ItemCarrinho(testProduct.Codigo, testProduct.Codigo, testProduct.Nome, testProduct.Preco, 1, testProduct.UrlImagem);
            carrinhoServiceMock
                .Setup(c => c.AddItem(clienteId, It.IsAny<ItemCarrinho>()))
                .ReturnsAsync(
                new CarrinhoCliente(clienteId,
                    new List<ItemCarrinho>
                    {
                        itemCarrinho
                    }))
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index(testProduct.Codigo);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            Assert.True(!string.IsNullOrWhiteSpace(controller.ViewBag.MsgServicoIndisponivel as string));
            catalogoServiceMock.Verify();
        }

        [Fact]
        public async Task Index_ProductNotFound()
        {
            //arrange
            var clienteId = "cliente_id";
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];
            catalogoServiceMock
                .Setup(c => c.GetProduto(testProduct.Codigo))
                .ReturnsAsync((Produto)null)
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index(testProduct.Codigo);

            //assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProdutoNaoEncontrado", redirectToActionResult.ActionName);
            Assert.Equal("Carrinho", redirectToActionResult.ControllerName);
            Assert.Equal(redirectToActionResult.Fragment, testProduct.Codigo);
            catalogoServiceMock.Verify();
        }
        #endregion

        #region UpdateQuantidade
        [Fact]
        public async Task UpdateQuantidade_Success()
        {
            //arrange
            var clienteId = "cliente_id";
            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);
            var itemCarrinho = GetFakeItemCarrinho();
            UpdateQuantidadeInput updateQuantidadeInput = new UpdateQuantidadeInput("001", 7);
            carrinhoServiceMock
                .Setup(c => c.UpdateItem(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ReturnsAsync(new UpdateQuantidadeOutput(itemCarrinho, new CarrinhoCliente()))
                .Verifiable();

            //act
            var result = await controller.UpdateQuantidade(updateQuantidadeInput);

            //assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UpdateQuantidadeOutput>(okObjectResult.Value);
            catalogoServiceMock.Verify();

        }

        [Fact]
        public async Task UpdateQuantidade_Invalid_ProdutoId()
        {
            //arrange
            var clienteId = "cliente_id";
            UpdateQuantidadeInput updateQuantidadeInput = new UpdateQuantidadeInput(null, 7);
            carrinhoServiceMock
                .Setup(c => c.UpdateItem(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ReturnsAsync(new UpdateQuantidadeOutput(new ItemCarrinho(), new CarrinhoCliente()))
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);
            controller.ModelState.AddModelError("ProdutoId", "Required");

            //act
            var result = await controller.UpdateQuantidade(updateQuantidadeInput);

            //assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestObjectResult.Value);
            catalogoServiceMock.Verify();

        }

        [Fact]
        public async Task UpdateQuantidade_ProdutoId_NotFound()
        {
            //arrange
            var clienteId = "cliente_id";
            UpdateQuantidadeInput updateQuantidadeInput = new UpdateQuantidadeInput("001", 7);
            carrinhoServiceMock
                .Setup(c => c.UpdateItem(clienteId, It.IsAny<UpdateQuantidadeInput>()))
                .ReturnsAsync((UpdateQuantidadeOutput)null)
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.UpdateQuantidade(updateQuantidadeInput);

            //assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(updateQuantidadeInput, notFoundObjectResult.Value);
            catalogoServiceMock.Verify();

        }
        #endregion

        #region Checkout (POST)
        [Fact]
        public async Task Checkout_success()
        {
            //arrange
            var carrinho = GetCarrinhoController();

            //act
            IActionResult actionResult = await carrinho.Checkout(new Cadastro());

            //assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
        }

        [Fact]
        public async Task Checkout_Invalid_Cadastro()
        {
            //arrange
            var carrinho = GetCarrinhoController();
            carrinho.ModelState.AddModelError("Email", "Required");

            //act
            IActionResult actionResult = await carrinho.Checkout(new Cadastro());

            //assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            redirectToActionResult.ControllerName = "CarrinhoController";
            redirectToActionResult.ActionName = "Checkout";
        }

        [Fact]
        public async Task Checkout_Service_Error()
        {
            //arrange
            carrinhoServiceMock
                .Setup(c => c.Checkout(It.IsAny<string>(), It.IsAny<CadastroViewModel>()))
                .ThrowsAsync(new Exception())
                .Verifiable();
            var controller = GetCarrinhoController();

            //act
            IActionResult actionResult = await controller.Checkout(new Cadastro());

            //assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            Assert.True(!string.IsNullOrWhiteSpace(controller.ViewBag.MsgServicoIndisponivel as string));
            catalogoServiceMock.Verify();

        }

        [Fact]
        public async Task Checkout_Service_BrokenCircuitException()
        {
            //arrange
            carrinhoServiceMock
                .Setup(c => c.Checkout(It.IsAny<string>(), It.IsAny<CadastroViewModel>()))
                .ThrowsAsync(new BrokenCircuitException())
                .Verifiable();
            var controller = GetCarrinhoController();

            //act
            IActionResult actionResult = await controller.Checkout(new Cadastro());

            //assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            Assert.True(!string.IsNullOrWhiteSpace(controller.ViewBag.MsgServicoIndisponivel as string));
            catalogoServiceMock.Verify();

        }
        #endregion

        #region Checkout (GET)
        [Fact]
        public async Task Checkout_Index_Get_Success()
        {
            //arrange
            appUserParserMock
                .Setup(a => a.Parse(It.IsAny<IPrincipal>()))
                .Returns(new ApplicationUser())
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser("001", controller);
            //act
            IActionResult actionResult = await controller.Checkout();

            //assert
            Assert.IsType<ViewResult>(actionResult);
            appUserParserMock.Verify();
        }

        [Fact]
        public async Task Checkout_Index_Get_Error()
        {
            //arrange
            appUserParserMock
                .Setup(a => a.Parse(It.IsAny<IPrincipal>()))
                .Returns((ApplicationUser)null)
                .Verifiable();

            var controller = GetCarrinhoController();
            SetControllerUser("001", controller);

            //act
            IActionResult actionResult = await controller.Checkout();

            //assert
            Assert.IsType<ViewResult>(actionResult);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            appUserParserMock.Verify();

        }
        #endregion

        private CarrinhoController GetCarrinhoController()
        {
            return new CarrinhoController(contextAccessorMock.Object, appUserParserMock.Object, loggerMock.Object, catalogoServiceMock.Object, carrinhoServiceMock.Object, userRedisRepositoryMock.Object, signalRClientMock.Object);
        }
    }
}
