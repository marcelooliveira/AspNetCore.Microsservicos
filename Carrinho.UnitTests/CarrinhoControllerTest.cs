//using CasaDoCodigo;
//using CasaDoCodigo.Controllers;
//using CasaDoCodigo.Models.ViewModels;
//using CasaDoCodigo.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;
//using CarrinhoClienteMVC = CasaDoCodigo.Models.ViewModels.CarrinhoCliente;

//namespace Carrinho.UnitTests
//{
//    public class CarrinhoControllerTest
//    {
//        //private readonly Mock<ICatalogoService> _catalogServiceMock;
//        //private readonly Mock<ICarrinhoService> _carrinhoServiceMock;
//        //private readonly Mock<IIdentityParser<ApplicationUser>> _identityParserMock;

//        private readonly Mock<HttpContext> _contextMock;
//        private readonly Mock<ILogger<PedidoController>> _loggerMock;
//        private readonly Mock<IHttpContextAccessor>  _contextAccessorMock;
//        private readonly Mock<HttpClient> _httpClientMock;
//        private readonly Mock<ISessionHelper>  _sessionHelperMock;
//        private readonly Mock<IConfiguration> _configurationMock;
//        private readonly Mock<ICatalogoService> _catalogoServiceMock;
//        private readonly Mock<ICarrinhoService> _carrinhoServiceMock;
//        private readonly Mock<IIdentityParser<ApplicationUser>> _appUserParserMock;

//        public CarrinhoControllerTest()
//        {
//            //_catalogServiceMock = new Mock<ICatalogoService>();
//            //_carrinhoServiceMock = new Mock<ICarrinhoService>();
//            //_identityParserMock = new Mock<IIdentityParser<ApplicationUser>>();

//            _contextMock = new Mock<HttpContext>();
//            _loggerMock             = new Mock<ILogger<PedidoController>>();
//            _contextAccessorMock    = new Mock<IHttpContextAccessor>();
//            _httpClientMock         = new Mock<HttpClient>();
//            _sessionHelperMock      = new Mock<ISessionHelper>();
//            _configurationMock      = new Mock<IConfiguration>();
//            _catalogoServiceMock    = new Mock<ICatalogoService>();
//            _carrinhoServiceMock    = new Mock<ICarrinhoService>();
//            _appUserParserMock      = new Mock<IIdentityParser<ApplicationUser>>();
//        }

//        [Fact]
//        public async Task Post_carrinho_sucesso()
//        {
//            //Arrange
//            var fakeClienteId = "1";
//            var action = string.Empty;
//            var fakeCarrinho = GetFakeCarrinho(fakeClienteId);
//            var fakeQuantidades = new Dictionary<string, int>()
//            {
//                ["produtoFakeA"] = 1,
//                ["produtoFakeB"] = 2
//            };

//            _carrinhoServiceMock.Setup(x => x.AtualizarQuantidades(It.IsAny<ApplicationUser>(), It.IsAny<Dictionary<string, int>>()))
//                .Returns(Task.FromResult(fakeCarrinho));

//            _carrinhoServiceMock.Setup(x => x.AtualizarCarrinho(It.IsAny<CarrinhoClienteMVC>()))
//                .Returns(Task.FromResult(fakeCarrinho));

//            //Act
//            var pedidoController = new PedidoController(
//                _loggerMock.Object,
//                _contextAccessorMock.Object,
//                _httpClientMock.Object,
//                _sessionHelperMock.Object,
//                _configurationMock.Object,
//                _catalogoServiceMock.Object,
//                _carrinhoServiceMock.Object,
//                _appUserParserMock.Object);

//            pedidoController.ControllerContext.HttpContext = _contextMock.Object;
//            var actionResult = await pedidoController..Index(fakeQuantidades, action);

//            //Assert
//            var viewResult = Assert.IsType<ViewResult>(actionResult);
//        }

//        //[Fact]
//        //public async Task Post_carrinho_checkout_sucesso()
//        //{
//        //    //Arrange
//        //    var fakeBuyerId = "1";
//        //    var action = "[ Checkout ]";
//        //    var fakeBasket = GetFakeBasket(fakeBuyerId);
//        //    var fakeQuantities = new Dictionary<string, int>()
//        //    {
//        //        ["produtoFakeA"] = 1,
//        //        ["produtoFakeB"] = 2
//        //    };

//        //    _basketServiceMock.Setup(x => x.SetQuantities(It.IsAny<ApplicationUser>(), It.IsAny<Dictionary<string, int>>()))
//        //        .Returns(Task.FromResult(fakeBasket));

//        //    _basketServiceMock.Setup(x => x.UpdateBasket(It.IsAny<BasketModel>()))
//        //        .Returns(Task.FromResult(fakeBasket));

//        //    //Act
//        //    var orderController = new CartController(_basketServiceMock.Object, _catalogServiceMock.Object, _identityParserMock.Object);
//        //    orderController.ControllerContext.HttpContext = _contextMock.Object;
//        //    var actionResult = await orderController.Index(fakeQuantities, action);

//        //    //Assert
//        //    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
//        //    Assert.Equal("Order", redirectToActionResult.ControllerName);
//        //    Assert.Equal("Create", redirectToActionResult.ActionName);
//        //}

//        //[Fact]
//        //public async Task Adicionar_ao_carrinho_sucesso()
//        //{
//        //    //Arrange
//        //    var fakeCatalogItem = GetFakeCatalogItem();

//        //    _basketServiceMock.Setup(x => x.AddItemToBasket(It.IsAny<ApplicationUser>(), It.IsAny<Int32>()))
//        //        .Returns(Task.FromResult(1));

//        //    //Act
//        //    var orderController = new CartController(_basketServiceMock.Object, _catalogServiceMock.Object, _identityParserMock.Object);
//        //    orderController.ControllerContext.HttpContext = _contextMock.Object;
//        //    var actionResult = await orderController.AddToCart(fakeCatalogItem);

//        //    //Assert
//        //    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
//        //    Assert.Equal("Catalog", redirectToActionResult.ControllerName);
//        //    Assert.Equal("Index", redirectToActionResult.ActionName);
//        //}

//        private CarrinhoClienteMVC GetFakeCarrinho(string clienteId)
//        {
//            return new CarrinhoClienteMVC()
//            {
//                ClienteId = clienteId
//            };
//        }

//        //private CatalogItem GetFakeItemCatalogo()
//        //{
//        //    return new Produto()
//        //    {
//        //        Id = 1,
//        //        Name = "fakeName",
//        //        CatalogBrand = "fakeBrand",
//        //        CatalogType = "fakeType",
//        //        CatalogBrandId = 2,
//        //        CatalogTypeId = 5,
//        //        Price = 20
//        //    };
//        //}
//    }
//}
