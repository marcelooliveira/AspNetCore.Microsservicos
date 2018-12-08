using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Model.Redis
{
    public class UserRedisRepository : IUserRedisRepository
    {
        private readonly ILogger<UserRedisRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public UserRedisRepository(ILogger<UserRedisRepository> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = redis.GetDatabase();
        }

        private IServer GetServer()
        {
            var endpoints = _redis.GetEndPoints();
            return _redis.GetServer(endpoints.First());
        }

        public async Task<int> GetUserNotificationCountAsync(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException();

            var data = await _database.StringGetAsync(clienteId);
            if (data.IsNullOrEmpty)
            {
                await UpdateUserNotificationCountAsync(clienteId, 0);
                return 0;
            }
            return JsonConvert.DeserializeObject<int>(data);
        }

        public async Task UpdateUserNotificationCountAsync(string clienteId, int userNotificationCount)
        {
            await _database.StringSetAsync(clienteId, userNotificationCount);
        }
    }
}
