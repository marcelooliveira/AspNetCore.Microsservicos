using Autofac;
using Autofac.Extensions.DependencyInjection;
using CasaDoCodigo.Carrinho.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Rebus.Bus;
using CasaDoCodigo.Mensagens.Events;
using Rebus.Config;

namespace CasaDoCodigo.Carrinho
{
    public class Startup
    {
        private const string RMQ_CONNECTION_STRING = "amqp://localhost";
        private const string INPUT_QUEUE_NAME = "CheckoutEvent";
        private readonly ILoggerFactory _loggerFactory;

        public Startup(ILoggerFactory loggerFactory, 
            IConfiguration configuration)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication()
                .AddJwtBearer(bearerOptions =>
                {
                    var paramsValidation = bearerOptions.TokenValidationParameters;
                    paramsValidation.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]));
                    paramsValidation.ValidAudience = Configuration["Tokens:Issuer"];
                    paramsValidation.ValidIssuer = Configuration["Tokens:Issuer"];

                    // Valida a assinatura de um token recebido
                    paramsValidation.ValidateIssuerSigningKey = true;

                    // Verifica se um token recebido ainda é válido
                    paramsValidation.ValidateLifetime = true;

                    // Tempo de tolerância para a expiração de um token (utilizado
                    // caso haja problemas de sincronismo de horário entre diferentes
                    // computadores envolvidos no processo de comunicação)
                    paramsValidation.ClockSkew = TimeSpan.Zero;
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Casa do Código - Carrinho",
                    Description = "Uma API contendo funcionalidades da carrinho de e-Commerce.",
                    TermsOfService = "Nenhum",
                    Contact = new Contact
                    {
                        Name = "Marcelo Oliveira",
                        Email = "mclricardo@gmail.com",
                        Url = "https://twitter.com/twmoliveira"
                    },
                    License = new License
                    {
                        Name = "Licença XPTO 4567",
                        Url = "https://example.com/license"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.ConfigureSwaggerGen(options =>
            {
                // UseFullTypeNameInSchemaIds replacement for .NET Core
                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddDistributedMemoryCache();
            services.AddSession();

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CarrinhoConfig>>().Value;
                //var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);
                var configuration = ConfigurationOptions.Parse("localhost", true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddTransient<ICarrinhoRepository, RedisCarrinhoRepository>();

            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            //ConfigureBrighter(services);
            ConfigureRebus(services);
        }

        private void ConfigureRebus(IServiceCollection services)
        {
            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq(RMQ_CONNECTION_STRING, INPUT_QUEUE_NAME)));
        }

        //private static void ConfigureBrighter(IServiceCollection services)
        //{
        //    //var container = new TinyIoCContainer();
        //    //var messageMapperFactory = new TinyIoCMessageMapperFactory(container);

        //    services.AddBrighter()
        //        .HandlersFromAssemblies(typeof(CheckoutEventHandler).Assembly)
        //        .Services.AddTransient<CheckoutEventMessageMapper>();

        //    //services.AddTransient<IAmAMessageMapper<CheckoutEvent>, CheckoutEventMessageMapper>();

        //    var container = services.BuildServiceProvider();
        //    var messageMapperFactory = new NETCoreIoCMessageMapperFactory();

        //    var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
        //    {
        //        {typeof(CheckoutEvent), typeof(CheckoutEventMessageMapper)}
        //    };

        //    var messageStore = new InMemoryMessageStore();
        //    var rmqConnnection = new RmqMessagingGatewayConnection
        //    {
        //        AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
        //        Exchange = new Exchange("paramore.brighter.exchange"),
        //    };
        //    var producer = new RmqMessageProducer(rmqConnnection);

        //    var builder = CommandProcessorBuilder.With()
        //        .Handlers(new HandlerConfiguration())
        //        .DefaultPolicy()
        //        .TaskQueues(new MessagingConfiguration(messageStore, producer, messageMapperRegistry))
        //        .RequestContextFactory(new InMemoryRequestContextFactory());

        //    var commandProcessor = builder.Build();
        //    services.AddSingleton(typeof(IAmACommandProcessor), commandProcessor);

        //    var newContainer = services.BuildServiceProvider();
        //    messageMapperFactory.Container = newContainer;
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseSession();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Casa do Código - Carrinho v1");
            });

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseRebus();
        }
    }
}
