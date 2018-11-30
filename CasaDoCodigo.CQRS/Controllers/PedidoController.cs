using CasaDoCodigo.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class PedidoController : BaseController
    {
        public PedidoController(ILogger<PedidoController> logger) : base(logger)
        {
        }

        public async Task<ActionResult> Historico()
        {
            return View();
        }
    }
}
