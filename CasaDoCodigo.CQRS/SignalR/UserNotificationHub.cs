using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC.SignalR
{
    public class UserNotificationHub : Hub
    {
        public Task SendUserNotification(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveUserNotification", message);
        }
    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
