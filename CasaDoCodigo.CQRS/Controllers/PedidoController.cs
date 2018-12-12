using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using MVC.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    [Route("[controller]")]
    public class PedidoController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;
        private readonly IPedidoService pedidoService;

        public PedidoController(
            IIdentityParser<ApplicationUser> appUserParser,
            IPedidoService pedidoService,
            ILogger<PedidoController> logger,
            IUserRedisRepository repository) 
            : base(logger, repository)
        {
            this.appUserParser = appUserParser;
            this.pedidoService = pedidoService;
        }

        [HttpGet("{clienteId}")]
        public async Task<ActionResult> Historico(string clienteId)
        {
            await CheckUserNotificationCount();

            List<PedidoDTO> model = await pedidoService.GetAsync(clienteId);
            return base.View(model);
        }
    }
}
