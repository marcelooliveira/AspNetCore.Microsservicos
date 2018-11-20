using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace MVC.Test
{
    public class CadastroControllerTest : BaseControllerTest
    {
        private readonly Mock<ILogger<CadastroController>> loggerMock;

        public CadastroControllerTest() : base()
        {
            loggerMock = new Mock<ILogger<CadastroController>>();
        }

        #region Index
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
                })
               .Verifiable();

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
            appUserParserMock.Verify();
        }

        [Fact]
        public async Task Index_No_User()
        {
            //arrange
            appUserParserMock
                .Setup(a => a.Parse(It.IsAny<IPrincipal>()))
                .Returns((ApplicationUser)null)
               .Verifiable();

            var controller =
                new CadastroController(appUserParserMock.Object, loggerMock.Object);

            SetControllerUser("001", controller);

            //act
            IActionResult result = await controller.Index();

            //assert
            Assert.IsType<ViewResult>(result);
            loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            appUserParserMock.Verify();

        }
        #endregion
    }
}
