using System.Threading.Tasks;

namespace MVC.SignalR
{
    public interface ISignalRClient
    {
        Task Start(string clientId);
    }
}