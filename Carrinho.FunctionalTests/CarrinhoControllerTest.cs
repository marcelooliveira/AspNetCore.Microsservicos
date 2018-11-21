using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Carrinho.FunctionalTests
{
    public class CarrinhoControllerTest
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(CarrinhoControllerTest))
               .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<CarrinhoTestStartup>();

            return new TestServer(hostBuilder);
        }

        [Fact]
        public async Task Get_carrinho_and_response_ok_status_codeAsync()
        {
            using (var server = new TestServer(builder))
            {
                var clienteId = "123";
                var client = server.CreateClient();
                var response = await client.GetAsync($"api/carrinho/{clienteId}");
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
