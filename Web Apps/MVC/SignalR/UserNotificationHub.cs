using MVC.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MVC.SignalR
{
    public class UserNotificationHub : Hub
    {
        private readonly IUserRedisRepository userRedisRepository;
        protected readonly ILogger logger;

        public UserNotificationHub(IUserRedisRepository userRedisRepository,
            ILogger<UserNotificationHub> logger)
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
            var userNotifications = await userRedisRepository.GetUnreadUserNotificationsAsync(user);
            await Task.Delay(1000);
            return userNotifications.Count;
        }
    }
}
