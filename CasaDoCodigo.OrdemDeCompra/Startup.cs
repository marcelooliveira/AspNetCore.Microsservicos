using CasaDoCodigo.Mensagens.EventHandling;
using CasaDoCodigo.Mensagens.Events;
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
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
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
        private readonly ILoggerFactory _loggerFactory;

        public Startup(ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _loggerFactory.AddDebug(); // logs to the debug output window in VS.
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

            //RegisterBrighter(services);
            //RegisterBrighter2(services);
            RegisterRebus(services);
        }

        private void RegisterRebus(IServiceCollection services)
        {
            // Register handlers 
            services.AutoRegisterHandlersFromAssemblyOf<CheckoutEventHandler>();

            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq("amqp://localhost", "Messages")))
                .AutoRegisterHandlersFromAssemblyOf<CheckoutEvent>();
        }

        //private static void RegisterBrighter(IServiceCollection services)
        //{
        //    Log.Logger = new LoggerConfiguration()
        //      .MinimumLevel.Debug()
        //      .WriteTo.LiterateConsole()
        //      .CreateLogger();

        //    services.AddScoped<IHandleRequests<CheckoutEvent>, CheckoutEventHandler>();

        //    services.AddBrighter()
        //        .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly)
        //        .Services.AddTransient<CheckoutEventMessageMapper>();

        //    var container = services.BuildServiceProvider();
        //    var handlerFactory = new NETCoreIoCHandlerFactory();
        //    var messageMapperFactory = new NETCoreIoCMessageMapperFactory();

        //    handlerFactory.Container = container;
        //    messageMapperFactory.Container = container;

        //    var subscriberRegistry = new SubscriberRegistry();
        //    subscriberRegistry.Register<CheckoutEvent, CheckoutEventHandler>();


        //    //create policies
        //    var retryPolicy = Policy
        //      .Handle<Exception>()
        //      .WaitAndRetry(new[]
        //      {
        //    TimeSpan.FromMilliseconds(50),
        //    TimeSpan.FromMilliseconds(100),
        //    TimeSpan.FromMilliseconds(150)
        //      });

        //    var circuitBreakerPolicy = Policy
        //      .Handle<Exception>()
        //      .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));

        //    var policyRegistry = new PolicyRegistry
        //{
        //  {CommandProcessor.RETRYPOLICY, retryPolicy},
        //  {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
        //};

        //    //create message mappers
        //    var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
        //{
        //  {typeof(CheckoutEvent), typeof(CheckoutEventMessageMapper)}
        //};

        //    //create the gateway
        //    var rmqConnnection = new RmqMessagingGatewayConnection
        //    {
        //        AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
        //        Exchange = new Exchange("paramore.brighter.exchange"),
        //    };

        //    var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnnection);

        //    var dispatcher = DispatchBuilder.With()
        //      .CommandProcessor(CommandProcessorBuilder.With()
        //        .Handlers(new HandlerConfiguration(subscriberRegistry, handlerFactory))
        //        .Policies(policyRegistry)
        //        .NoTaskQueues()
        //        .RequestContextFactory(new InMemoryRequestContextFactory())
        //        .Build())
        //      .MessageMappers(messageMapperRegistry)
        //      .DefaultChannelFactory(new InputChannelFactory(rmqMessageConsumerFactory))
        //      .Connections(new Connection[]
        //      {
        //    new Connection<CheckoutEvent>(
        //      new ConnectionName("paramore.example.greeting"),
        //      new ChannelName("greeting.event"),
        //      new RoutingKey("greeting.event"),
        //      timeoutInMilliseconds: 200,
        //      isDurable: true,
        //      highAvailability: true)
        //      }).Build();

        //    dispatcher.Receive();

        //    dispatcher.End().Wait();

        //}

        //private static void RegisterBrighter2(IServiceCollection services)
        //{
        //    Log.Logger = new LoggerConfiguration()
        //    .MinimumLevel.Debug()
        //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        //    .Enrich.FromLogContext()
        //    .WriteTo.Console()
        //    .CreateLogger();

        //    services.AddScoped<IHandleRequests<CheckoutEvent>, CheckoutEventHandler>();

        //    //services.AddBrighter()
        //    //    .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly)
        //    //    .Services.AddTransient<CheckoutEventMessageMapper>();

        //    var container = services.BuildServiceProvider();
        //    var handlerFactory = new NETCoreIoCHandlerFactory();
        //    var messageMapperFactory = new NETCoreIoCMessageMapperFactory();

        //    handlerFactory.Container = container;
        //    messageMapperFactory.Container = container;

        //    var subscriberRegistry = new SubscriberRegistry();
        //    subscriberRegistry.Register<CheckoutEvent, CheckoutEventHandler>();

        //    //create policies
        //    var retryPolicy = Policy
        //      .Handle<Exception>()
        //      .WaitAndRetry(new[]
        //      {
        //    TimeSpan.FromMilliseconds(50),
        //    TimeSpan.FromMilliseconds(100),
        //    TimeSpan.FromMilliseconds(150)
        //      });

        //    var circuitBreakerPolicy = Policy
        //      .Handle<Exception>()
        //      .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));

        //    var policyRegistry = new PolicyRegistry
        //    {
        //      {CommandProcessor.RETRYPOLICY, retryPolicy},
        //      {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
        //    };

        //    var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
        //    {
        //      {typeof(CheckoutEvent), typeof(CheckoutEventMessageMapper)}
        //    };

        //    var messagingConfiguration =
        //        new MessagingConfiguration(
        //            new InMemoryMessageStore(), messageProducer: null, messageMapperRegistry);

        //    //var host = new HostBuilder()
        //    //    .ConfigureServices((hostContext, services) =>
        //    //    {
        //    var connections = new Connection[]
        //            {
        //                new Connection<CheckoutEvent>(
        //                    new ConnectionName("paramore.example.greeting"),
        //                    new ChannelName("greeting.event"),
        //                    new RoutingKey("greeting.event"),
        //                    timeoutInMilliseconds: 200,
        //                    isDurable: true,
        //                    highAvailability: true)
        //            };

        //            var rmqConnnection = new RmqMessagingGatewayConnection
        //            {
        //                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
        //                Exchange = new Exchange("paramore.brighter.exchange")
        //            };

        //            var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnnection);

        //            services.AddServiceActivator(options =>
        //            {
        //                options.Connections = connections;
        //                options.ChannelFactory = new InputChannelFactory(rmqMessageConsumerFactory);
        //                options.PolicyRegistry = policyRegistry;
        //                options.RequestContextFactory =
        //                        new InMemoryRequestContextFactory();
        //                options.MessagingConfiguration = messagingConfiguration;
        //            })
        //            .MapperRegistryFromAssemblies(typeof(CheckoutEventHandler).Assembly)
        //            .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly, typeof(ExceptionPolicyHandler<>).Assembly);

        //    services.AddSingleton<ILoggerFactory>(x => new SerilogLoggerFactory());
        //            services.AddHostedService<MyServiceActivatorHostedService>();
        //    services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

        //    //})
        //    //.UseConsoleLifetime()
        //    //.Build();

        //    //await host.RunAsync();
        //}

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
            app.UseRebus(async bus => await bus.Subscribe<CheckoutEvent>());
            //serviceProvider.GetService<ApplicationContext>().Database.MigrateAsync().Wait();
        }
    }
}
