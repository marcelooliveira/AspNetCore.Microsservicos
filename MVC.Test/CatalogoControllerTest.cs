using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Services;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CatalogoControllerTest : BaseControllerTest
    {
        private readonly Mock<ICatalogoService> catalogoServiceMock;
        private readonly Mock<ILogger<CatalogoController>> loggerMock;

        public CatalogoControllerTest()
        {
            catalogoServiceMock = new Mock<ICatalogoService>();
            loggerMock = new Mock<ILogger<CatalogoController>>();
        }

        [Fact]
        public async Task Index_sucesso()
        {
            //arrange
            IList<Produto> fakeProdutos = GetFakeProdutos();
            catalogoServiceMock
                .Setup(s => s.GetProdutos())
                .ReturnsAsync(fakeProdutos);

            //act
            var catalogoController = 
                new CatalogoController(catalogoServiceMock.Object, loggerMock.Object);
            var resultado = await catalogoController.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsAssignableFrom<IList<Produto>>(viewResult.ViewData.Model);

            Assert.Equal(fakeProdutos[0].Codigo, model[0].Codigo);
            Assert.Equal(fakeProdutos[1].Codigo, model[1].Codigo);
            Assert.Equal(fakeProdutos[2].Codigo, model[2].Codigo);
        }

        [Fact]
        public async Task Index_BrokenCircuitException()
        {
            //arrange
            catalogoServiceMock
                .Setup(s => s.GetProdutos())
                .ThrowsAsync(new BrokenCircuitException());

            //act
            var catalogoController =
                new CatalogoController(catalogoServiceMock.Object, loggerMock.Object);

            var result = await catalogoController.Index();
            var model = result as IList<Produto>;

            //assert
            Assert.Null(model);
            Assert.True(!string.IsNullOrWhiteSpace(catalogoController.ViewBag.MsgServicoIndisponivel));
        }

        [Fact]
        public async Task Index_Exception()
        {
            //arrange
            catalogoServiceMock
                .Setup(s => s.GetProdutos())
                .ThrowsAsync(new Exception());

            //act
            var catalogoController =
                new CatalogoController(catalogoServiceMock.Object, loggerMock.Object);

            var result = await catalogoController.Index();
            var model = result as IList<Produto>;

            //assert
            Assert.Null(model);
            Assert.True(!string.IsNullOrWhiteSpace(catalogoController.ViewBag.MsgServicoIndisponivel));
        }
    }
}
