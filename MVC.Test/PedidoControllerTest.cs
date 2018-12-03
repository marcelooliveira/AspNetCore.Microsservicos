using AutoMapper;
using AutoMapper.Configuration;
using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrdemDeCompra.API.Models;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class PedidoControllerTest : BaseControllerTest
    {
        private readonly Mock<ILogger<PedidoController>> loggerMock;
        private readonly Mock<IPedidoService> pedidoServiceMock;

        public PedidoControllerTest() :base()
        {
            loggerMock = new Mock<ILogger<PedidoController>>();
            pedidoServiceMock = new Mock<IPedidoService>();

            var mappings = new MapperConfigurationExpression();
            mappings.AddProfile<MappingProfile>();
            Mapper.Initialize(mappings);
        }

        [Fact]
        public async Task Historico_Ok()
        {
            //arrange
            appUserParserMock
                .Setup(a => a.Parse(It.IsAny<IPrincipal>()))
                .Returns(new ApplicationUser())
                .Verifiable();

            string clienteId = "123";
            List<ItemPedidoDTO> itens = new List<ItemPedidoDTO> {
                new ItemPedidoDTO("001", "produto 001", 1, 12.34m)
            };
            PedidoDTO pedido = new PedidoDTO(itens, "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            pedidoServiceMock
                .Setup(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync( new List<PedidoDTO> { pedido })
                .Verifiable();

            var controller = new PedidoController(appUserParserMock.Object
                , pedidoServiceMock.Object
                , loggerMock.Object);
            SetControllerUser(clienteId, controller);

            //act
            ActionResult actionResult = await controller.Historico();

            //assert
            ViewResult viewResult = Assert.IsAssignableFrom<ViewResult>(actionResult);
            List<PedidoDTO>  pedidos = Assert.IsType<List<PedidoDTO>>(viewResult.Model);
            Assert.Collection(pedidos[0].Itens,
                i => Assert.Equal("001", i.ProdutoCodigo));
            pedidoServiceMock.Verify();
        }
    }
}
