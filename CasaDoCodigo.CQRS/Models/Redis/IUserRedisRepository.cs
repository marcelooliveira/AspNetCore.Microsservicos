using System.Threading.Tasks;

namespace MVC.Model.Redis
{
    public interface IUserRedisRepository
    {
        Task<int> GetUserNotificationCountAsync(string clienteId);
        Task UpdateUserNotificationCountAsync(string clienteId, int userNotificationCount);
        Task IncrementUserNotificationCountAsync(string clienteId);
    }
}