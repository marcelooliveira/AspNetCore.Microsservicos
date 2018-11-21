using Carrinho.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Carrinho.FunctionalTests
{
    public class CarrinhoTestStartup : Startup
    {
        public CarrinhoTestStartup(ILoggerFactory loggerFactory,
            IConfiguration configuration) : base(loggerFactory, configuration)
        {
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}