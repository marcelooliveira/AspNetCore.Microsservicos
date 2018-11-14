using Carrinho.API.Model;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carrinho.API.Tests
{
    public class RedisCarrinhoRepositoryTest
    {
        private readonly Mock<ILogger<RedisCarrinhoRepository>> loggerMock;
        private readonly Mock<ConnectionMultiplexer> redisMock;

        #region GetCarrinhoAsync
        public async Task GetCarrinhoAsync_success()
        {
            //arrange
            string clienteId = "123";
            var databaseMock = 
                new Mock<IDatabase>()
                .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue());

            //redisMock
            //    .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            //    .Returns("");

            var repository 
                = new RedisCarrinhoRepository(loggerMock.Object, redisMock.Object);

            //act
            var carrinhoCliente = await repository.GetCarrinhoAsync(clienteId);

            //assert

        }
        #endregion

        #region GetUsuarios

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
