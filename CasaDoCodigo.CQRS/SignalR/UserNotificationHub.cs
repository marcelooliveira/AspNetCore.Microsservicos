using CasaDoCodigo.Models;
using Microsoft.AspNetCore.SignalR;
using MVC.Model.Redis;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MVC.SignalR
{
    public class UserNotificationHub : Hub
    {
        private readonly IUserRedisRepository userRedisRepository;

        public UserNotificationHub(IUserRedisRepository userRedisRepository)
        {
            this.userRedisRepository = userRedisRepository;
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
            var userNotifications = await userRedisRepository.GetUserNotificationsAsync(user);
            await Task.Delay(3000);
            return userNotifications.Count;
        }
    }
}
