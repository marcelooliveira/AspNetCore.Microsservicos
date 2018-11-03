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

            var seed = args.Any(x => x == "/seed");
            if (seed) args = args.Except(new[] { "/seed" }).ToArray();

            var host = BuildWebHost(args);

            if (seed)
            {
                await SeedData.EnsureSeedData(host.Services);
                return;
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"OrdemDeCompra_log.txt")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            return 
                WebHost.CreateDefaultBuilder(args)
                        .UseStartup<Startup>()
                            .ConfigureLogging(builder =>
                            {
                                builder.ClearProviders();
                                builder.AddSerilog();
                            })
                        .Build();
        }
    }
}
