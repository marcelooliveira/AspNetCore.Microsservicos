using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;

namespace Identity.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Identity.API";
            var host = BuildWebHost(args);
            SeedData.EnsureSeedData(host.Services);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .Build();
        }
    }
}
