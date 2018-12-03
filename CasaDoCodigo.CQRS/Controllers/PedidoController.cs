using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class PedidoController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;
        private readonly IPedidoService pedidoService;

        public PedidoController(
            IIdentityParser<ApplicationUser> appUserParser,
            IPedidoService pedidoService,
            ILogger<PedidoController> logger) : base(logger)
        {
            this.appUserParser = appUserParser;
            this.pedidoService = pedidoService;
        }

        public async Task<ActionResult> Historico()
        {
            var usuario = appUserParser.Parse(HttpContext.User);
            List<PedidoDTO> model = await pedidoService.GetAsync(usuario.Id);
            return base.View(model);
        }
    }
}
