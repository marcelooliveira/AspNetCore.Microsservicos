using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Catalogo.UnitTests
{
    public class CatalogoControllerTest
    {
        private readonly Mock<ICatalogoService> _catalogoServiceMock;
        private readonly Mock<ILogger<CatalogoController>> _loggerMock;

        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public CatalogoControllerTest()
        {
            _catalogoServiceMock = new Mock<ICatalogoService>();
            _loggerMock = new Mock<ILogger<CatalogoController>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public async Task Get_itens_catalogo_sucesso()
        {
            //Arrange
            IList<Produto> fakeProdutos = GetFakeCatalogo();

            _catalogoServiceMock
                .Setup(c => c.GetProdutos())
                .Returns(Task.FromResult(fakeProdutos));

            _loggerMock
                .Setup(m => m.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            _httpContextAccessorMock
                .Setup(_ => _.HttpContext)
                .Returns(new DefaultHttpContext());

            //Act
            var _catalogoController = new CatalogoController(_catalogoServiceMock.Object, _loggerMock.Object, _httpContextAccessorMock.Object);
            var actionResult = await _catalogoController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);
            var model = Assert.IsAssignableFrom<IList<Produto>>(viewResult.ViewData.Model);

            Assert.Equal(fakeProdutos[0].Codigo, model[0].Codigo);
            Assert.Equal(fakeProdutos[1].Codigo, model[1].Codigo);
            Assert.Equal(fakeProdutos[2].Codigo, model[2].Codigo);
        }

        private static IList<Produto> GetFakeCatalogo()
        {
            return new List<Produto>
            {
                new Produto(1, "prod1", "produto 1", 12.34m),
                new Produto(2, "prod2", "produto 2", 23.45m),
                new Produto(3, "prod3", "produto 3", 34.56m)
            };
        }
    }
}
