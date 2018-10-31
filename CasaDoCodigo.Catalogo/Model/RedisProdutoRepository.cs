using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API.Model
{
    public class RedisProdutoRepository
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisProdutoRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }
    }
}
