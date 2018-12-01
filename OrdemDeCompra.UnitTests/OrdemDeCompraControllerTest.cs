using CasaDoCodigo.OrdemDeCompra.Controllers;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrdemDeCompra.UnitTests
{
    public class OrdemDeCompraControllerTest
    {
        Mock<IPedidoRepository> pedidoRepositoryMock;

        public OrdemDeCompraControllerTest()
        {
            pedidoRepositoryMock = new Mock<IPedidoRepository>();
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
            Pedido pedido = new Pedido(new List<ItemPedido>(), clienteId, clienteNome, clienteEmail, clienteTelefone, clienteEndereco, clienteComplemento, clienteBairro, clienteMunicipio, clienteUF, clienteCEP);
            var controller = new OrdemDeCompraController(pedidoRepositoryMock.Object);
            controller.ModelState.AddModelError("cliente", "Required");
            //act
            IActionResult actionResult = await controller.Post(pedido);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }
    }
}
