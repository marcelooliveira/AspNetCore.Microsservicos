using CasaDoCodigo.Models;
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
        private const int USER_DB_INDEX = 1;
        private readonly ILogger<UserRedisRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public UserRedisRepository(ILogger<UserRedisRepository> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = redis.GetDatabase(USER_DB_INDEX);
        }

        private IServer GetServer()
        {
            var endpoints = _redis.GetEndPoints();
            return _redis.GetServer(endpoints.First());
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException();

            var data = await _database.StringGetAsync(clienteId);
            if (data.IsNullOrEmpty)
            {
                var userNotifications = new List<UserNotification>();
                await UpdateUserNotificationAsync(clienteId, userNotifications);
                return new List<UserNotification>();
            }
            return JsonConvert.DeserializeObject<List<UserNotification>>(data);
        }

        public async Task AddUserNotificationAsync(string clienteId, UserNotification userNotification)
        {
            var userNotifications = await GetUserNotificationsAsync(clienteId);
            userNotifications.Add(userNotification);
            await UpdateUserNotificationAsync(clienteId, userNotifications);
        }

        private async Task UpdateUserNotificationAsync(string clienteId, List<UserNotification> userNotifications)
        {
            var json = JsonConvert.SerializeObject(userNotifications);
            await _database.StringSetAsync(clienteId, json);
        }
    }
}
