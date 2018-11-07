using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CadastroControllerTest
    {
        private readonly Mock<IIdentityParser<ApplicationUser>> appUserParserMock;
        private readonly Mock<ILogger<CadastroController>> loggerMock;
        private readonly Mock<HttpContext> contextMock;

        public CadastroControllerTest()
        {
            appUserParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            loggerMock = new Mock<ILogger<CadastroController>>();
            contextMock = new Mock<HttpContext>();
        }

        [Fact]
        public async Task Index_success()
        {
            //arrange
            appUserParserMock
                .Setup(aup => aup.Parse(It.IsAny<ClaimsPrincipal>()))
                .Returns(new ApplicationUser
                {
                    Bairro = "bbb",
                    CEP = "ccc",
                    Complemento = "ccc",
                    Email = "eee",
                    Endereco = "eee",
                    Municipio = "mmm",
                    Nome = "nnn",
                    Telefone = "ttt",
                    UF = "uuu"
                });

            //act
            var cadastroController = 
                new CadastroController(appUserParserMock.Object, loggerMock.Object);
            cadastroController.ControllerContext.HttpContext = contextMock.Object;
            var result = await cadastroController.Index();

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CadastroViewModel>(viewResult.ViewData.Model);
            Assert.Equal<CadastroViewModel>(model,
                new CadastroViewModel
                {
                    Bairro = "bbb",
                    CEP = "ccc",
                    Complemento = "ccc",
                    Email = "eee",
                    Endereco = "eee",
                    Municipio = "mmm",
                    Nome = "nnn",
                    Telefone = "ttt",
                    UF = "uuu"
                });
        }
    }
}
