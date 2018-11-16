using Catalogo.API.Controllers;
using Catalogo.API.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Catalogo.UnitTests
{
    public class UnitTest1
    {
        private readonly Mock<ILogger<ProdutoController>> loggerMock;
        private readonly Mock<IProdutoQueries> produtoQueriesMock;

        public UnitTest1()
        {
            this.loggerMock = new Mock<ILogger<ProdutoController>>();
            this.produtoQueriesMock = new Mock<IProdutoQueries>();
        }

        [Fact]
        public async Task GetProdutos_success()
        {
            //arrange
            var controller = new ProdutoController(loggerMock.Object, produtoQueriesMock.Object);

            //act
            var actionResult = await controller.GetProdutos();

            //assert
            Assert.IsType<OkResult>(actionResult);
        }
    }
}
