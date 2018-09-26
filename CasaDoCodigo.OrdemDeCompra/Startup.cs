using CasaDoCodigo.Mensagens;
using CasaDoCodigo.Mensagens.Adapters.ServiceHost;
using CasaDoCodigo.OdemDeCompra.IntegrationEvents.EventHandling;
using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Events;
using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Mappers;
using CasaDoCodigo.OrdemDeCompra.Commands;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.AspNetCore;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Brighter.Policies.Handlers;
using Paramore.Brighter.ServiceActivator;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Polly;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CasaDoCodigo.OdemDeCompra
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDistributedMemoryCache();
            services.AddSession();

            string connectionString = Configuration.GetConnectionString("Default");

            services.AddScoped<DbContext, ApplicationContext>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddScoped<IMediator, NoMediator>();
            //services.AddScoped<IRequestHandler<IdentifiedCommand<CreatePedidoCommand, bool>, bool>, CreatePedidoCommandHandler>();
            services.AddScoped<IRequest<bool>, CreatePedidoCommand>();
            services.AddMediatR(typeof(CreatePedidoCommand).GetTypeInfo().Assembly);

            RegisterBrighter(services);
            //RegisterBrighter2(services);
        }

        private static void RegisterBrighter(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.LiterateConsole()
              .CreateLogger();

            services.AddScoped<IHandleRequests<CheckoutEvent>, CheckoutEventHandler>();

            services.AddBrighter()
                .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly)
                .Services.AddTransient<CheckoutEventMessageMapper>();

            var container = services.BuildServiceProvider();
            var handlerFactory = new NETCoreIoCHandlerFactory();
            var messageMapperFactory = new NETCoreIoCMessageMapperFactory();

            handlerFactory.Container = container;
            messageMapperFactory.Container = container;

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.Register<CheckoutEvent, CheckoutEventHandler>();


            //create policies
            var retryPolicy = Policy
              .Handle<Exception>()
              .WaitAndRetry(new[]
              {
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(150)
              });

            var circuitBreakerPolicy = Policy
              .Handle<Exception>()
              .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));

            var policyRegistry = new PolicyRegistry
        {
          {CommandProcessor.RETRYPOLICY, retryPolicy},
          {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
        };

            //create message mappers
            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
        {
          {typeof(CheckoutEvent), typeof(CheckoutEventMessageMapper)}
        };

            //create the gateway
            var rmqConnnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                Exchange = new Exchange("paramore.brighter.exchange"),
            };

            var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnnection);

            var dispatcher = DispatchBuilder.With()
              .CommandProcessor(CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(subscriberRegistry, handlerFactory))
                .Policies(policyRegistry)
                .NoTaskQueues()
                .RequestContextFactory(new InMemoryRequestContextFactory())
                .Build())
              .MessageMappers(messageMapperRegistry)
              .DefaultChannelFactory(new InputChannelFactory(rmqMessageConsumerFactory))
              .Connections(new Connection[]
              {
            new Connection<CheckoutEvent>(
              new ConnectionName("paramore.example.greeting"),
              new ChannelName("greeting.event"),
              new RoutingKey("greeting.event"),
              timeoutInMilliseconds: 200,
              isDurable: true,
              highAvailability: true)
              }).Build();

            dispatcher.Receive();

            dispatcher.End().Wait();

        }

        private static void RegisterBrighter2(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            services.AddScoped<IHandleRequests<CheckoutEvent>, CheckoutEventHandler>();

            //services.AddBrighter()
            //    .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly)
            //    .Services.AddTransient<CheckoutEventMessageMapper>();

            var container = services.BuildServiceProvider();
            var handlerFactory = new NETCoreIoCHandlerFactory();
            var messageMapperFactory = new NETCoreIoCMessageMapperFactory();

            handlerFactory.Container = container;
            messageMapperFactory.Container = container;

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.Register<CheckoutEvent, CheckoutEventHandler>();

            //create policies
            var retryPolicy = Policy
              .Handle<Exception>()
              .WaitAndRetry(new[]
              {
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(150)
              });

            var circuitBreakerPolicy = Policy
              .Handle<Exception>()
              .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));

            var policyRegistry = new PolicyRegistry
            {
              {CommandProcessor.RETRYPOLICY, retryPolicy},
              {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
            };

            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
              {typeof(CheckoutEvent), typeof(CheckoutEventMessageMapper)}
            };

            var messagingConfiguration =
                new MessagingConfiguration(
                    new InMemoryMessageStore(), messageProducer: null, messageMapperRegistry);

            //var host = new HostBuilder()
            //    .ConfigureServices((hostContext, services) =>
            //    {
            var connections = new Connection[]
                    {
                        new Connection<CheckoutEvent>(
                            new ConnectionName("paramore.example.greeting"),
                            new ChannelName("greeting.event"),
                            new RoutingKey("greeting.event"),
                            timeoutInMilliseconds: 200,
                            isDurable: true,
                            highAvailability: true)
                    };

                    var rmqConnnection = new RmqMessagingGatewayConnection
                    {
                        AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                        Exchange = new Exchange("paramore.brighter.exchange")
                    };

                    var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnnection);

                    services.AddServiceActivator(options =>
                    {
                        options.Connections = connections;
                        options.ChannelFactory = new InputChannelFactory(rmqMessageConsumerFactory);
                        options.PolicyRegistry = policyRegistry;
                        //options.RequestContextFactory =
                        //        new InMemoryRequestContextFactory();
                        //options.MessagingConfiguration = messagingConfiguration;
                    })
                    .MapperRegistryFromAssemblies(typeof(CheckoutEventHandler).Assembly)
                    .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly, typeof(ExceptionPolicyHandler<>).Assembly);

            services.AddSingleton<ILoggerFactory>(x => new SerilogLoggerFactory());
                    services.AddHostedService<MyServiceActivatorHostedService>();
            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
            //})
            //.UseConsoleLifetime()
            //.Build();

            //await host.RunAsync();
        }

        public void Configure(IServiceProvider serviceProvider, IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            //serviceProvider.GetService<ApplicationContext>().Database.MigrateAsync().Wait();
        }
    }

    public class MyServiceActivatorHostedService : IHostedService
    {
        private IServiceProvider Services { get; }
        private readonly ILogger<ServiceActivatorHostedService> _logger;
        private readonly IDispatcher _dispatcher;


        public MyServiceActivatorHostedService(IServiceProvider services, 
            ILogger<ServiceActivatorHostedService> logger, IDispatcher dispatcher)
        {
            Services = services;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting hosted service dispatcher");

            //_dispatcher.Receive();

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IScopedProcessingService>();

                scopedProcessingService.DoWork();
            }

            var completionSource = new TaskCompletionSource<IDispatcher>();
            completionSource.SetResult(_dispatcher);

            return completionSource.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping hosted service dispatcher");
            return _dispatcher.End();
        }
    }

    internal interface IScopedProcessingService
    {
        void DoWork();
    }

    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IDispatcher _dispatcher;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger,
            IDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public void DoWork()
        {
            _dispatcher.Receive();
            //_dispatcher.End().Wait();

            _logger.LogInformation("Scoped Processing Service is working.");
        }
    }
}
