using Carrinho.API.Model;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Carrinho.API.Tests
{
    public class RedisCarrinhoRepositoryTest
    {
        private readonly Mock<ILogger<RedisCarrinhoRepository>> loggerMock;
        private readonly Mock<IConnectionMultiplexer> redisMock;

        public RedisCarrinhoRepositoryTest()
        {
            loggerMock = new Mock<ILogger<RedisCarrinhoRepository>>();
            redisMock = new Mock<IConnectionMultiplexer>();
        }

        #region GetCarrinhoAsync
        [Fact]
        public async Task GetCarrinhoAsync_success()
        {
            //arrange
            var json = @"{
                  ""ClienteId"": ""123"",
                  ""Itens"": [{
                  ""Id"": ""001"",
                  ""ProdutoId"": ""001"",
                  ""ProdutoNome"": ""Produto 001"",
                  ""Quantidade"": 7,
                  ""PrecoUnitario"": 12.34}]
                }";

            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(json);

            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object);

            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            var carrinhoCliente = await repository.GetCarrinhoAsync(clienteId);

            //assert
            Assert.Equal(clienteId, carrinhoCliente.ClienteId);
            Assert.Collection(carrinhoCliente.Itens,
                item =>
                {
                    Assert.Equal("001", item.ProdutoId);
                    Assert.Equal(7, item.Quantidade);
                });
        }

        [Fact]
        public async Task GetCarrinhoAsync_invalid_clienteId()
        {
            //arrange
            string clienteId = "";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act - assert
            await Assert.ThrowsAsync<ArgumentException>(() 
                => repository.GetCarrinhoAsync(clienteId));
        }

        [Fact]
        public async Task GetCarrinhoAsync_clienteId_NotFound()
        {
            //arrange
            var json = @"{
                  ""ClienteId"": ""123"",
                  ""Itens"": []
                }";

            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        It.IsAny<RedisValue>(),
                        null,
                        When.Always,
                        CommandFlags.None
                    ))
               .ReturnsAsync(true);

            databaseMock.SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync("")
                    .ReturnsAsync(json);


            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object);

            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            var carrinhoCliente = await repository.GetCarrinhoAsync(clienteId);

            //assert
            Assert.Equal(clienteId, carrinhoCliente.ClienteId);
            Assert.Empty(carrinhoCliente.Itens);
        }
        #endregion

        #region GetUsuarios

        public async Task GetUsuarios_success()
        {
            //arrange
            var repository
                    = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            var usuarios = repository.GetUsuarios();

            //assert
        }
        #endregion

        #region GetCarrinhoAsync

        #endregion

        #region GetUsuarios

        #endregion

        #region UpdateCarrinhoAsync

        #endregion

        #region AddCarrinhoAsync

        #endregion

        #region UpdateCarrinhoAsync

        #endregion

        #region DeleteCarrinhoAsync

        #endregion
    }
}
