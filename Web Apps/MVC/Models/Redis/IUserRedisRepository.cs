using MVC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC.Model.Redis
{
    public interface IUserRedisRepository
    {
        Task<UserCounterData> GetUserCounterDataAsync(string clienteId);
        Task AddUserNotificationAsync(string clienteId, UserNotification userNotification);
        Task UpdateUserBasketCountAsync(string clienteId, int userBasketCount);
        Task MarkAllAsReadAsync(string clienteId);
    }
}