using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Catalogo.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = "Catalogo.API";

            IWebHost host = BuildWebHost(args);
            await SeedData.EnsureSeedData(host.Services);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost
                    .CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .Build();
        }
    }
}
