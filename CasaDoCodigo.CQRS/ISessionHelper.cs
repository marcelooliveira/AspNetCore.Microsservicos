using System.Threading.Tasks;

namespace CasaDoCodigo
{
    public interface ISessionHelper
    {
        Task<string> GetAccessToken(string scope);
        int? GetPedidoId();
        void SetAccessToken(string accessToken);
        void SetPedidoId(int pedidoId);
    }
}