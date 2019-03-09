using MVC.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MVC.SignalR
{
    public class UserCounterDataHub : Hub
    {
        private readonly IUserRedisRepository userRedisRepository;
        protected readonly ILogger logger;

        public UserCounterDataHub(IUserRedisRepository userRedisRepository,
            ILogger<UserCounterDataHub> logger)
        {
            this.userRedisRepository = userRedisRepository;
            this.logger = logger;
        }

        public async Task SendUserNotification(string user, string message)
        {
            int count = await UpdateUserNotifications(user, message);
            await Clients.User(user).SendAsync("ReceiveMessage", user, count);
        }

        private async Task<int> UpdateUserNotifications(string user, string message)
        {
            var userNotification = new UserNotification(user, message, DateTime.Now, null);
            await userRedisRepository.AddUserNotificationAsync(user, userNotification);
            var userCounterData = await userRedisRepository.GetUserCounterDataAsync(user);
            await Task.Delay(1000);
            return userCounterData.Notifications.Count;
        }

        public async Task UpdateUserBasketCount(string user, int basketCount)
        {
            await userRedisRepository.UpdateUserBasketCountAsync(user, basketCount);
            await Clients.User(user).SendAsync("ReceiveUserBasketCount", user, basketCount);
        }
    }
}
