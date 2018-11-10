using CasaDoCodigo;
using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CarrinhoClienteMVC = CasaDoCodigo.Models.ViewModels.CarrinhoCliente;

namespace Carrinho.UnitTests
{
    public class CarrinhoControllerTest
    {
        private readonly Mock<HttpContext> _contextMock;
        private readonly ILogger<CarrinhoController> _logger;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly Mock<ISessionHelper> _sessionHelperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ICatalogoService> _catalogoServiceMock;
        private readonly Mock<ICarrinhoService> _carrinhoServiceMock;
        private readonly Mock<IIdentityParser<ApplicationUser>> _appUserParserMock;

        public CarrinhoControllerTest()
        {
            _contextMock = new Mock<HttpContext>();
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpClientMock = new Mock<HttpClient>();
            _sessionHelperMock = new Mock<ISessionHelper>();
            _configurationMock = new Mock<IConfiguration>();
            _catalogoServiceMock = new Mock<ICatalogoService>();
            _carrinhoServiceMock = new Mock<ICarrinhoService>();
            _appUserParserMock = new Mock<IIdentityParser<ApplicationUser>>();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();
            _logger = factory.CreateLogger<CarrinhoController>();
        }

        [Fact]
        public async Task Post_carrinho_sucesso()
        {
            //Arrange
            var fakeClienteId = "1";
            var action = string.Empty;
            var fakeCarrinho = GetFakeCarrinho(fakeClienteId);
            var fakeQuantidades = new Dictionary<string, int>()
            {
                ["produtoFakeA"] = 1,
                ["produtoFakeB"] = 2
            };

            _carrinhoServiceMock.Setup(x => x.DefinirQuantidades(It.IsAny<ApplicationUser>(), It.IsAny<Dictionary<string, int>>()))
                .Returns(Task.FromResult(fakeCarrinho));

            _carrinhoServiceMock.Setup(x => x.AtualizarCarrinho(It.IsAny<CarrinhoClienteMVC>()))
                .Returns(Task.FromResult(fakeCarrinho));

            var carrinhoController = new CarrinhoController(
                _contextAccessorMock.Object,
                _appUserParserMock.Object,
                _logger,
                _catalogoServiceMock.Object,
                _carrinhoServiceMock.Object);
            carrinhoController.ControllerContext.HttpContext = _contextMock.Object;

            //Act
            var actionResult = await carrinhoController.Index(fakeQuantidades, action);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);
        }

        private CarrinhoClienteMVC GetFakeCarrinho(string clienteId)
        {
            return new CarrinhoClienteMVC()
            {
                ClienteId = clienteId
            };
        }
    }
}
