using CasaDoCodigo.OrdemDeCompra;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OrdemDeCompra.UnitTests
{
    public class PedidoRepositoryTest
    {
        private readonly Mock<ApplicationContext> contextoMock;

        public PedidoRepositoryTest()
        {
            this.contextoMock = new Mock<ApplicationContext>();
        }

        [Fact]
        public async Task CreateOrUpdate_Pedido_NullAsync()
        {
            //arrange
            var repo = new PedidoRepository(contextoMock.Object);

            //act+assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.CreateOrUpdate(null));
        }
    }
}
