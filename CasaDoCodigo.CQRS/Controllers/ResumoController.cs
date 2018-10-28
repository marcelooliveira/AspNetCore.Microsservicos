using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CasaDoCodigo.Controllers
{
    public class ResumoController : BaseController
    {
        private readonly IIdentityParser<ApplicationUser> appUserParser;

        public IConfiguration Configuration { get; }

        public ResumoController(
            IHttpContextAccessor contextAccessor,
            IIdentityParser<ApplicationUser> appUserParser
            )
            : base(contextAccessor)
        {
            this.appUserParser = appUserParser;
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Resumo(Cadastro cadastro)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var viewModel = new CasaDoCodigo.Models.CadastroViewModel(cadastro);
        //        viewModel.PedidoId = GetPedidoId().Value;
        //        var pedidoViewModel = await apiService.UpdateCadastro(viewModel);
        //        return base.View(pedidoViewModel);
        //    }
        //    return RedirectToAction("Cadastro");
        //}
    }
}
