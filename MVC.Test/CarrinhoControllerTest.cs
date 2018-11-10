using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CarrinhoControllerTest : BaseControllerTest
    {
        private readonly Mock<IHttpContextAccessor> contextAccessorMock;
        private readonly Mock<IIdentityParser<ApplicationUser>> appUserParserMock;
        private readonly Mock<ILogger<CarrinhoController>> loggerMock;
        private readonly Mock<ICatalogoService> catalogoServiceMock;
        private readonly Mock<ICarrinhoService> carrinhoServiceMock;

        public CarrinhoControllerTest()
        {
            contextAccessorMock = new Mock<IHttpContextAccessor>();
            appUserParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            loggerMock = new Mock<ILogger<CarrinhoController>>();
            catalogoServiceMock = new Mock<ICatalogoService>();
            carrinhoServiceMock = new Mock<ICarrinhoService>();
        }

        [Fact]
        public async Task CarrinhoController_Index_Success()
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

            var controller = new CarrinhoController(contextAccessorMock.Object, appUserParserMock.Object, loggerMock.Object, catalogoServiceMock.Object, carrinhoServiceMock.Object);
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index(testProduct.Codigo);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarrinhoCliente>(viewResult.Model);
            Assert.Equal(model.Itens[0].ProdutoNome, produtos[0].Nome);
        }

        [Fact]
        public async Task CarrinhoController_Index_Without_Product_Success()
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

            var controller = new CarrinhoController(contextAccessorMock.Object, appUserParserMock.Object, loggerMock.Object, catalogoServiceMock.Object, carrinhoServiceMock.Object);
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarrinhoCliente>(viewResult.Model);
            Assert.Equal(model.Itens[0].ProdutoNome, produtos[0].Nome);
        }

        [Fact]
        public async Task CarrinhoController_Index_BrokenCircuitException()
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

            var controller = new CarrinhoController(contextAccessorMock.Object, appUserParserMock.Object, loggerMock.Object, catalogoServiceMock.Object, carrinhoServiceMock.Object);
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index(testProduct.Codigo);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task CarrinhoController_ProductNotFound()
        {
            //arrange
            var clienteId = "cliente_id";
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];
            catalogoServiceMock
                .Setup(c => c.GetProduto(testProduct.Codigo))
                .ReturnsAsync((Produto)null)
                .Verifiable();

            var controller = new CarrinhoController(contextAccessorMock.Object, appUserParserMock.Object, loggerMock.Object, catalogoServiceMock.Object, carrinhoServiceMock.Object);
            SetControllerUser(clienteId, controller);

            //act
            var result = await controller.Index(testProduct.Codigo);

            //assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProdutoNaoEncontrado", redirectToActionResult.ActionName);
            Assert.Equal("Carrinho", redirectToActionResult.ControllerName);
            Assert.Equal(redirectToActionResult.Fragment, testProduct.Codigo);
        }

        private static void SetControllerUser(string clienteId, CarrinhoController controller)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { new Claim("sub", clienteId) }
                ));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
