using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CasaDoCodigo.Mensagens;
using CasaDoCodigo.Mensagens.Ports.Commands;
using CasaDoCodigo.OdemDeCompra.IntegrationEvents.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using NServiceBus.Features;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDistributedMemoryCache();
            services.AddSession();

            string connectionString = Configuration.GetConnectionString("Default");

            services.AddScoped<DbContext, ApplicationContext>();
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            //configure autofac
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);

            //containerBuilder.RegisterModule(new MediatorModule());
            //containerBuilder.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            // NServiceBus
            var container = RegisterEventBus(containerBuilder);

            return new AutofacServiceProvider(container);
        }

        IContainer RegisterEventBus(ContainerBuilder containerBuilder)
        {
            //EnsureSqlDatabaseExists();

            IEndpointInstance endpoint = null;
            containerBuilder.Register(c => endpoint)
                .As<IEndpointInstance>()
                .SingleInstance();

            var container = containerBuilder.Build();

            var endpointConfiguration = new EndpointConfiguration("Ordering");

            // Configure RabbitMQ transport
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString(GetRabbitConnectionString());

            //// Configure persistence
            //var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            //persistence.SqlDialect<SqlDialect.MsSqlServer>();
            //persistence.ConnectionBuilder(connectionBuilder:
            //    () => new SqlConnection(Configuration["ConnectionString"]));

            // Use JSON.NET serializer
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            //// Enable the Outbox.
            //endpointConfiguration.EnableOutbox();
            endpointConfiguration.DisableFeature<Outbox>();

            // Make sure NServiceBus creates queues in RabbitMQ, tables in SQL Server, etc.
            // You might want to turn this off in production, so that DevOps can use scripts to create these.
            endpointConfiguration.EnableInstallers();

            // Turn on auditing.
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            // Define conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("IntegrationEvent"));
            conventions.DefiningCommandsAs(c => c.Namespace != null && c.Namespace.EndsWith("Messages") && c.Name.EndsWith("Command"));

            // Configure the DI container.
            endpointConfiguration.UseContainer<AutofacBuilder>(customizations: customizations => { customizations.ExistingLifetimeScope(container); });

            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            endpoint.Subscribe(typeof(CheckoutEvent)).Wait();

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

            serviceProvider.GetService<ApplicationContext>().Database.MigrateAsync().Wait();
        }
    }
}
