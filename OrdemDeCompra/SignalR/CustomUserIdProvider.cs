using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace OrdemDeCompra.API.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
