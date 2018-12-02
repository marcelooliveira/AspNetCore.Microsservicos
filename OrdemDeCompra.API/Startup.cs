using AutoMapper;
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
using Rebus.ServiceProvider;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace CasaDoCodigo.OrdemDeCompra
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
            _loggerFactory.AddDebug(); // logs to the debug output window in VS.
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Casa do Código - Ordem de Compra API",
                    Description = "Uma API contendo funcionalidades da aplicação de e-Commerce:" +
                    "Criação de pedidos.",
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

            string connectionString = Configuration.GetConnectionString("Default");

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddScoped<DbContext, ApplicationContext>();
            var serviceProvider = services.BuildServiceProvider();
            var contexto = serviceProvider.GetService<ApplicationContext>();
            services.AddSingleton<ApplicationContext>(contexto);

            services.AddScoped<IPedidoRepository, PedidoRepository>();

            services.AddScoped<IMediator, NoMediator>();
            services.AddScoped<IRequest<bool>, CreatePedidoCommand>();
            services.AddMediatR(typeof(CreatePedidoCommand).GetTypeInfo().Assembly);
            RegisterRebus(services);

        }

        private void RegisterRebus(IServiceCollection services)
        {
            // Register handlers 
            services.AutoRegisterHandlersFromAssemblyOf<CheckoutEventHandler>();

            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq(RMQ_CONNECTION_STRING, INPUT_QUEUE_NAME)))
                .AddTransient<DbContext, ApplicationContext>()
                .AutoRegisterHandlersFromAssemblyOf<CheckoutEvent>();
        }

        public void Configure(IServiceProvider serviceProvider, IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            app.UseRebus(
                async (bus) =>
                {
                    await bus.Subscribe<CheckoutEvent>();
                });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Casa do Código - Ordem de Compra v1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
