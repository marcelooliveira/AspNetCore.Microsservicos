using CasaDoCodigo.Controllers;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MVC.Test
{
    public class BaseControllerTest
    {
        protected readonly Mock<IHttpContextAccessor> contextAccessorMock;
        protected readonly Mock<IIdentityParser<ApplicationUser>> appUserParserMock;
        protected readonly Mock<HttpContext> contextMock;

        public BaseControllerTest()
        {
            this.contextAccessorMock = new Mock<IHttpContextAccessor>();
            this.appUserParserMock = new Mock<IIdentityParser<ApplicationUser>>();
            this.contextMock = new Mock<HttpContext>();
        }

        protected ItemCarrinho GetFakeItemCarrinho()
        {
            var produtos = GetFakeProdutos();
            var testProduct = produtos[0];
            var itemCarrinho = new ItemCarrinho(testProduct.Codigo, testProduct.Codigo, testProduct.Nome, testProduct.Preco, 7, testProduct.UrlImagem);
            return itemCarrinho;
        }

        protected IList<Produto> GetFakeProdutos()
        {
            return new List<Produto>
            {
                new Produto("001", "produto 001", 12.34m),
                new Produto("002", "produto 002", 23.45m),
                new Produto("003", "produto 003", 34.56m)
            };
        }

        protected static void SetControllerUser(string clienteId, BaseController controller)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { new Claim("sub", clienteId) }
                ));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
