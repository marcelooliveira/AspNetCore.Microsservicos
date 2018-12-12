using Microsoft.AspNetCore.SignalR;
using MVC.Model.Redis;
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
            await userRedisRepository.IncrementUserNotificationCountAsync(user);
            await Clients.User(user).SendAsync("ReceiveMessage", message);
        }
    }
}
