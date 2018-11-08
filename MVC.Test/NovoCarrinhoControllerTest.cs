using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class NovoCarrinhoControllerTest : BaseControllerTest
    {
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<IIdentityParser<ApplicationUser>> appUserParser;
        private readonly Mock<ILogger<CarrinhoController>> logger;
        private readonly Mock<ICatalogoService> catalogoService;
        private readonly Mock<ICarrinhoService> carrinhoService;

        public NovoCarrinhoControllerTest()
        {
            contextAccessor = new Mock<IHttpContextAccessor>();
            appUserParser = new Mock<IIdentityParser<ApplicationUser>>();
            logger = new Mock<ILogger<CarrinhoController>>();
            catalogoService = new Mock<ICatalogoService>();
            carrinhoService = new Mock<ICarrinhoService>();
        }

       [Fact]
        public async Task CarrinhoController_Index()
        {
            //arrange
            var produtos = GetFakeProdutos();

            //act
            var controller = new CarrinhoController(contextAccessor.Object, appUserParser.Object, logger.Object, catalogoService.Object, carrinhoService.Object);
            var testProduct = produtos[0];
            var result = await controller.Index(testProduct.Codigo);

            //assert
            Assert.IsType<ViewResult>(result);
        }       
    }
}
