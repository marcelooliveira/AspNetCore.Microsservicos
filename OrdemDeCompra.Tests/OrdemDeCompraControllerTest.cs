using AutoMapper;
using CasaDoCodigo.OrdemDeCompra.Controllers;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Models.DTOs;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrdemDeCompra.API.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace OrdemDeCompra.UnitTests
{
    public class OrdemDeCompraControllerTest
    {
        private readonly Mock<IPedidoRepository> pedidoRepositoryMock;
        private readonly IMapper mapper;

        public OrdemDeCompraControllerTest()
        {
            pedidoRepositoryMock = new Mock<IPedidoRepository>();
            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            mapper = mockMapper.CreateMapper();
        }

        [Theory]
        [InlineData("", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "", "complemento", "bairro", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "", "municipio", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "", "uf", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "", "12345-678")]
        [InlineData("clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "")]
        public async Task Post_Invalid_Pedido(string clienteId, string clienteNome, string clienteEmail, string clienteTelefone, string clienteEndereco, string clienteComplemento, string clienteBairro, string clienteMunicipio, string clienteUF, string clienteCEP)
        {
            //arrange
            List<ItemPedido> itens = new List<ItemPedido> {
                new ItemPedido("001", "produto 001", 1, 12.34m)
            };
            Pedido pedido = new Pedido(itens, clienteId, clienteNome, clienteEmail, clienteTelefone, clienteEndereco, clienteComplemento, clienteBairro, clienteMunicipio, clienteUF, clienteCEP);
            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            controller.ModelState.AddModelError("cliente", "Required");
            //act
            IActionResult actionResult = await controller.Post(pedido);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Post_Invalid_Pedido_No_Items()
        {
            //arrange
            Pedido pedido = new Pedido(new List<ItemPedido>(), "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            controller.ModelState.AddModelError("cliente", "Required");
            //act
            IActionResult actionResult = await controller.Post(pedido);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Post_Invalid_Pedido_Items_Null()
        {
            //arrange
            Pedido pedido = new Pedido(null, "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            controller.ModelState.AddModelError("cliente", "Required");
            //act
            IActionResult actionResult = await controller.Post(pedido);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Post_Invalid_Pedido_Success()
        {
            //arrange
            List<ItemPedido> itens = new List<ItemPedido> {
                new ItemPedido("001", "produto 001", 1, 12.34m)
            };
            Pedido pedido = new Pedido(itens, "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            pedido.Id = 123;
            pedidoRepositoryMock
                .Setup(r => r.CreateOrUpdate(It.IsAny<Pedido>()))
                .ReturnsAsync(pedido);
            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            //act
            IActionResult actionResult = await controller.Post(pedido);

            //assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            Pedido pedidoCriado = Assert.IsType<Pedido>(okObjectResult.Value);
            Assert.Equal(123, pedidoCriado.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Get_Invalid_ClienteId(string clienteId)
        {
            //arrange
            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            SetControllerUser(clienteId, controller);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.Get(clienteId));

        }

        [Fact]
        public async Task Get_Not_Found()
        {
            //arrange
            pedidoRepositoryMock
                .Setup(r => r.GetPedidos(It.IsAny<string>()))
                .ReturnsAsync((List<Pedido>)null)
                .Verifiable();

            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            SetControllerUser("xpto", controller);

            //act
            ActionResult result = await controller.Get("xpto");

            //assert
            Assert.IsType<NotFoundObjectResult>(result);

            pedidoRepositoryMock.Verify();
        }

        [Fact]
        public async Task Get_Ok()
        {
            //arrange
            List<ItemPedido> itens = new List<ItemPedido> {
                new ItemPedido("001", "produto 001", 1, 12.34m)
            };
            Pedido pedido = new Pedido(itens, "clienteId", "clienteNome", "cliente@email.com", "fone", "endereco", "complemento", "bairro", "municipio", "uf", "12345-678");
            pedido.Id = 123;

            pedidoRepositoryMock
                .Setup(r => r.GetPedidos(It.IsAny<string>()))
                .ReturnsAsync(new List<Pedido> { pedido })
                .Verifiable();

            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object, mapper);
            SetControllerUser("xpto", controller);

            //act
            ActionResult result = await controller.Get("xpto");

            //assert
            var objectResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var pedidos = Assert.IsType<List<PedidoDTO>>(objectResult.Value);
            Assert.Collection(pedidos,
                (p) => Assert.Equal("123", p.Id));

            Assert.Collection(pedidos[0].Itens,
                (i) => Assert.Equal("001", i.ProdutoCodigo));

            pedidoRepositoryMock.Verify();
        }

        protected static void SetControllerUser(string clienteId, ControllerBase controller)
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
