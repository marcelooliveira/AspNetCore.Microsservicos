using Autofac;
using Autofac.Extensions.DependencyInjection;
using Carrinho.API.Model;
using Carrinho.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Rebus.Config;
using Rebus.ServiceProvider;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Carrinho.API
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
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = "Carrinho.API";
                    options.ApiSecret = "secret";
                    options.Authority = Configuration["IdentityUrl"];
                    //options.BackchannelHttpHandler = new HttpClientHandler() { Proxy = new WebProxy(Configuration["System:Proxy"]) };
                    options.RequireHttpsMetadata = false;
                });

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

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CarrinhoConfig>>().Value;
                var configuration = ConfigurationOptions.Parse(Configuration["RedisConnectionString"], true);
                //var configuration = ConfigurationOptions.Parse("ecommerce.data:6379", true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddTransient<ICarrinhoRepository, RedisCarrinhoRepository>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            ConfigureRebus(services);
        }

        private void ConfigureRebus(IServiceCollection services)
        {
            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq(Configuration["RabbitMQConnectionString"], Configuration["RabbitMQInputQueueName"])));
        }

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
