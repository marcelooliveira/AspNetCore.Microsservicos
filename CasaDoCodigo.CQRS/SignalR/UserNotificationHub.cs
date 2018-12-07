using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC.SignalR
{
    public class UserNotificationHub : Hub
    {
        public async Task SendUserNotification(string user, string message)
        {
            //Clients.User(user).SendAsync("ReceiveUserNotification", message);
            await Task.Delay(2000);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
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
