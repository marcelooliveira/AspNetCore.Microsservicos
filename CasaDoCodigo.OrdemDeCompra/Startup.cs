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
using Paramore.Brighter;
using Paramore.Brighter.AspNetCore;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Brighter.ServiceActivator;
using Polly;
using Serilog;
using System;
using System.Reflection;

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
        }

        private static void RegisterBrighter(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.LiterateConsole()
              .CreateLogger();

            //var container = new TinyIoCContainer();
            //var handlerFactory = new TinyIocHandlerFactory(container);
            //var messageMapperFactory = new TinyIoCMessageMapperFactory(container);
            //container.Register<IHandleRequests<CheckoutEvent>, CheckoutEventHandler>();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IServiceProvider serviceProvider, IApplicationBuilder app, IHostingEnvironment env)
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
}
