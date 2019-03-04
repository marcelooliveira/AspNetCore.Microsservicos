using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Services;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MVC.Model.Redis;
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
        private readonly Mock<IUserRedisRepository> userRedisRepositoryMock;

        public CatalogoControllerTest() : base()
        {
            catalogoServiceMock = new Mock<ICatalogoService>();
            loggerMock = new Mock<ILogger<CatalogoController>>();
            userRedisRepositoryMock = new Mock<IUserRedisRepository>(); ;
    }

    [Fact]
        public async Task Index_sucesso()
        {
            //arrange
            IList<Produto> fakeProdutos = GetFakeProdutos();
            catalogoServiceMock
                .Setup(s => s.GetProdutos())
                .ReturnsAsync(fakeProdutos)
               .Verifiable();

            var catalogoController = 
                new CatalogoController(catalogoServiceMock.Object, loggerMock.Object, userRedisRepositoryMock.Object);

            //act
            var resultado = await catalogoController.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsAssignableFrom<IList<Produto>>(viewResult.ViewData.Model);

            Assert.Collection(model,
                               item => Assert.Equal(fakeProdutos[0].Codigo, item.Codigo),
                               item => Assert.Equal(fakeProdutos[1].Codigo, item.Codigo),
                               item => Assert.Equal(fakeProdutos[2].Codigo, item.Codigo)
                );
            catalogoServiceMock.Verify();
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
                new CatalogoController(catalogoServiceMock.Object, loggerMock.Object, userRedisRepositoryMock.Object);

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
                new CatalogoController(catalogoServiceMock.Object, loggerMock.Object, userRedisRepositoryMock.Object);

            var result = await catalogoController.Index();
            var model = result as IList<Produto>;

            //assert
            Assert.Null(model);
            Assert.True(!string.IsNullOrWhiteSpace(catalogoController.ViewBag.MsgServicoIndisponivel));
        }
    }
}
