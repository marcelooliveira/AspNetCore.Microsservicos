using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CasaDoCodigo.Carrinho.IntegrationEvents;
using CasaDoCodigo.Carrinho.Model;
using CasaDoCodigo.Mensagens;
using CasaDoCodigo.Mensagens.Adapters.ServiceHost;
using CasaDoCodigo.Mensagens.Ports.Commands;
using CasaDoCodigo.Mensagens.Ports.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NServiceBus;
using NServiceBus.Features;
using Paramore.Brighter;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;

namespace CasaDoCodigo.Carrinho
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
            services.AddAuthentication()
                //.AddJwtBearer(cfg =>
                //{
                //    cfg.RequireHttpsMetadata = false;
                //    cfg.SaveToken = true;

                //    cfg.TokenValidationParameters = new TokenValidationParameters()
                //    {
                //        ValidIssuer = Configuration["Tokens:Issuer"],
                //        ValidAudience = Configuration["Tokens:Issuer"],
                //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                //    };

                //});
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

            //// NServiceBus
            //var container = RegisterEventBus(containerBuilder);

            //return new AutofacServiceProvider(container);

            ConfigureBrighter(services);
        }

        private static void ConfigureBrighter(IServiceCollection services)
        {
            var container = new TinyIoCContainer();

            var messageMapperFactory = new TinyIoCMessageMapperFactory(container);

            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory)
            {
                {typeof(CheckoutEvent), typeof(CheckoutEventMessageMapper)}
            };

            var messageStore = new InMemoryMessageStore();
            var rmqConnnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                Exchange = new Exchange("paramore.brighter.exchange"),
            };
            var producer = new RmqMessageProducer(rmqConnnection);

            var builder = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration())
                .DefaultPolicy()
                .TaskQueues(new MessagingConfiguration(messageStore, producer, messageMapperRegistry))
                .RequestContextFactory(new InMemoryRequestContextFactory());

            var commandProcessor = builder.Build();
            services.AddSingleton(typeof(IAmACommandProcessor), commandProcessor);
        }

        private IContainer RegisterEventBus(ContainerBuilder containerBuilder)
        {
            //EnsureSqlDatabaseExists();

            IEndpointInstance endpoint = null;
            containerBuilder.Register(c => endpoint)
                .As<IEndpointInstance>()
                .SingleInstance();

            var container = containerBuilder.Build();

            var endpointConfiguration = new EndpointConfiguration("Carrinho");

            // Configure RabbitMQ transport
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString(GetRabbitConnectionString());

            // Configure persistence
            //var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            //persistence.SqlDialect<SqlDialect.MsSqlServer>();
            //persistence.ConnectionBuilder(connectionBuilder:
            //    () => new SqlConnection(Configuration["SqlConnectionString"]));

            // Use JSON.NET serializer
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            //// Enable the Outbox
            //endpointConfiguration.EnableOutbox();

            // Disable the Outbox
            endpointConfiguration.DisableFeature<Outbox>();

            // Make sure NServiceBus creates queues in RabbitMQ, tables in SQL Server, etc.
            // You might want to turn this off in production, so that DevOps can use scripts to create these.
            endpointConfiguration.EnableInstallers();

            // Turn on auditing.
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            // Define conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("IntegrationEvent"));

            // Configure the DI container.
            endpointConfiguration.UseContainer<AutofacBuilder>(customizations: customizations =>
            {
                customizations.ExistingLifetimeScope(container);
            });

            // Start the endpoint and register it with ASP.NET Core DI
            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            return container;
        }

        private string GetRabbitConnectionString()
        {
            var host = Configuration["EventBusConnection"];
            var user = Configuration["EventBusUserName"];
            var password = Configuration["EventBusPassword"];

            if (string.IsNullOrEmpty(user))
                return $"host={host}";

            return $"host={host};username={user};password={password};";
        }
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
        }
    }
}
