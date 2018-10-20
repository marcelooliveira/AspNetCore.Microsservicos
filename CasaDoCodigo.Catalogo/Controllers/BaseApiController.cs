using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.Catalogo.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected readonly ILogger logger;

        public BaseApiController(ILogger logger)
        {
            this.logger = logger;
        }
    }
}
