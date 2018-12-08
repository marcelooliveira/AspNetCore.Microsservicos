using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC.SignalR
{
    public class UserNotificationHub : Hub
    {
        public async Task SendUserNotification(string user, string message)
        {
            //await Task.Delay(5000);
            await Clients.User(user).SendAsync("ReceiveMessage", message);
        }
    }
}
