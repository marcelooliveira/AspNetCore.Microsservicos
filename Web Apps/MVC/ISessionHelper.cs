using System.Threading.Tasks;

namespace MVC
{
    public interface ISessionHelper
    {
        Task<string> GetAccessToken(string scope);
        int? GetPedidoId();
        void SetAccessToken(string accessToken);
        void SetPedidoId(int pedidoId);
    }
}