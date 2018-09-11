using CasaDoCodigo.Mensagens;
using CasaDoCodigo.Mensagens.Adapters.ServiceHost;
using CasaDoCodigo.Mensagens.Ports.CommandHandlers;
using CasaDoCodigo.Mensagens.Ports.Commands;
using CasaDoCodigo.Mensagens.Ports.Mappers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Paramore.Brighter;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Brighter.ServiceActivator;
using Polly;
using Serilog;
using System;

namespace CasaDoCodigo.OdemDeCompra
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
