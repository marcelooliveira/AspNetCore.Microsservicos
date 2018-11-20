using Carrinho.API.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                .ReturnsAsync(json)
                .Verifiable();
            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();

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

            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task GetCarrinhoAsync_invalid_clienteId()
        {
            //arrange
            string clienteId = "";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act - assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => repository.GetCarrinhoAsync(clienteId));
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
               .ReturnsAsync(true)
               .Verifiable();

            databaseMock.SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                    .ReturnsAsync("")
                    .ReturnsAsync(json);                 


            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            var carrinhoCliente = await repository.GetCarrinhoAsync(clienteId);

            //assert
            Assert.Equal(clienteId, carrinhoCliente.ClienteId);
            Assert.Empty(carrinhoCliente.Itens);
            databaseMock.Verify();
            redisMock.Verify();
        }
        #endregion

        #region AddCarrinhoAsync
        [Fact]
        public async Task AddCarrinhoAsync_success()
        {
            //arrange
            var json1 = JsonConvert.SerializeObject(new CarrinhoCliente("123") { Itens = new List<ItemCarrinho> { new ItemCarrinho("001", "001", "produto 001", 12.34m, 1) }});
            var json2 = JsonConvert.SerializeObject(new CarrinhoCliente("123") { Itens = new List<ItemCarrinho> { new ItemCarrinho("001", "001", "produto 001", 12.34m, 1), new ItemCarrinho("002", "002", "produto 002", 12.34m, 2) } });

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
            databaseMock
                .SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync("")
                .ReturnsAsync(json1)
                .ReturnsAsync(json2);

            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();

            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            ItemCarrinho item = new ItemCarrinho("002", "002", "produto 002", 12.34m, 2);

            //act
            var carrinhoCliente = await repository.AddCarrinhoAsync(clienteId, item);

            //assert
            Assert.Equal(clienteId, carrinhoCliente.ClienteId);
            Assert.Collection(carrinhoCliente.Itens,
                i =>
                {
                    Assert.Equal("001", i.ProdutoId);
                    Assert.Equal(1, i.Quantidade);
                },
                i =>
                {
                    Assert.Equal("002", i.ProdutoId);
                    Assert.Equal(2, i.Quantidade);
                });
            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task AddCarrinhoAsync_invalid_item()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => repository.AddCarrinhoAsync(clienteId, null));
        }

        [Fact]
        public async Task AddCarrinhoAsync_invalid_item2()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => repository.AddCarrinhoAsync(clienteId, new ItemCarrinho() { ProdutoId = "" }));
        }


        [Fact]
        public async Task AddCarrinhoAsync_negative_qty()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => repository.AddCarrinhoAsync(clienteId, new ItemCarrinho() { ProdutoId = "001", Quantidade = -1 }));
        }
        #endregion

        #region UpdateCarrinhoAsync
        [Fact]
        public async Task UpdateCarrinhoAsync_success()
        {
            //arrange
            var json1 = JsonConvert.SerializeObject(new CarrinhoCliente("123") { Itens = new List<ItemCarrinho> { new ItemCarrinho("001", "001", "produto 001", 12.34m, 1) } });
            var json2 = JsonConvert.SerializeObject(new CarrinhoCliente("123") { Itens = new List<ItemCarrinho> { new ItemCarrinho("001", "001", "produto 001", 12.34m, 2) } });

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
               .ReturnsAsync(true)
               .Verifiable();
            databaseMock
                .SetupSequence(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync("")
                .ReturnsAsync(json1)
                .ReturnsAsync(json2);

            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
               .Verifiable();

            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            var item = new UpdateQuantidadeInput("001", 2);

            //act
            var output = await repository.UpdateCarrinhoAsync(clienteId, item);

            //assert
            Assert.Equal(clienteId, output.CarrinhoCliente.ClienteId);
            Assert.Collection(output.CarrinhoCliente.Itens,
                i =>
                {
                    Assert.Equal("001", i.ProdutoId);
                    Assert.Equal(2, i.Quantidade);
                });

            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task UpdateCarrinhoAsync_invalid_item()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => repository.UpdateCarrinhoAsync(clienteId, null));
        }

        [Fact]
        public async Task UpdateCarrinhoAsync_invalid_item2()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => repository.UpdateCarrinhoAsync(clienteId, new UpdateQuantidadeInput() { ProdutoId = "" }));
        }


        [Fact]
        public async Task UpdateCarrinhoAsync_negative_qty()
        {
            //arrange
            string clienteId = "123";
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            //assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => repository.UpdateCarrinhoAsync(clienteId, new UpdateQuantidadeInput () { ProdutoId = "001", Quantidade = -1 }));
        }
        #endregion

        #region DeleteCarrinhoAsync
        [Fact]
        public async Task DeleteCarrinhoAsync_success()
        {
            //arrange
            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true)
                .Verifiable();
            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
                .Verifiable();
            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            bool result = await repository.DeleteCarrinhoAsync(clienteId);

            //assert
            Assert.True(result);
            databaseMock.Verify();
            redisMock.Verify();
        }

        [Fact]
        public async Task DeleteCarrinhoAsync_failure()
        {
            //arrange
            string clienteId = "123";
            var databaseMock = new Mock<IDatabase>();
            databaseMock
                .Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(false)
               .Verifiable();
            redisMock
                .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(databaseMock.Object)
               .Verifiable();

            var repository
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            bool result = await repository.DeleteCarrinhoAsync(clienteId);

            //assert
            Assert.False(result);
            databaseMock.Verify();
            redisMock.Verify();
        }
        #endregion
    }
}
