using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalogo.API.Controllers
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
