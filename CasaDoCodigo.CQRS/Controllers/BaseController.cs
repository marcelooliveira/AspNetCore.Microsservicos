using Microsoft.AspNetCore.Mvc;

namespace CasaDoCodigo.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void HandleBrokenCircuitException()
        {
            ViewBag.MsgCarrosselIndisponivel = "O serviço de carrossel não está ativo, por favor tente novamente mais tarde.";
        }
    }
}
