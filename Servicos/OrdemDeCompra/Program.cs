using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OrdemDeCompra.API;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "OrdemDeCompra.API";
            var host = BuildWebHost(args);
            await SeedData.EnsureSeedData(host.Services);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return
                WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
