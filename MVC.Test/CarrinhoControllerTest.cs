using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CarrinhoControllerTest
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
        public async Task Index_success()
        {
            var produtos = GetFakeProdutos();

            //arrange
            string userId = "user_id";

            catalogoServiceMock
                .Setup(c => c.GetProduto(produtos[0].Codigo))
                .ReturnsAsync(produtos[0]);

            carrinhoServiceMock
                .Setup(c => c.AddItem(userId, It.IsAny<ItemCarrinho>()))
                .ReturnsAsync(new CarrinhoCliente
                {
                     ClienteId = userId,
                     Itens = new List<ItemCarrinho>
                     {
                         new ItemCarrinho()
                     }
                });

            //act
            var carrinhoController = GetCarrinhoController();
            SetupUser(carrinhoController, userId, "user_name");

            var result = await carrinhoController.Index(produtos[0].Codigo);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarrinhoCliente>(viewResult);
        }

        private CarrinhoController GetCarrinhoController()
        {
            return new CarrinhoController(
            contextAccessorMock.Object,
            appUserParserMock.Object,
            loggerMock.Object,
            catalogoServiceMock.Object,
            carrinhoServiceMock.Object);
        }

        private IList<Produto> GetFakeProdutos()
        {
            return new List<Produto>
            {
                new Produto("001", "produto 001", 12.34m),
                new Produto("002", "produto 002", 23.45m),
                new Produto("003", "produto 003", 34.56m)
            };
        }

        private void SetupUser(Controller controller, string userId, string username)
        {
            var mockContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockContext.SetupGet(hc => hc.User.Identity.Name).Returns(username);
            mockContext.SetupGet(hc => hc.User.Claims).Returns(new List<Claim> { new Claim("sub", userId) });
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
        }
    }
}
