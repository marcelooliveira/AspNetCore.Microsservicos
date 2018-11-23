using CasaDoCodigo.Identity.API;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using Identity.API;
using Identity.API.Commands;
using Identity.API.IntegrationEvents.EventHandling;
using Identity.API.Managers;
using Identity.API.Models;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewIdentity.Data;
using Rebus.Config;
using Rebus.ServiceProvider;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace NewIdentity
{
    public class Startup
    {
        private const string RMQ_CONNECTION_STRING = "amqp://localhost";
        private const string INPUT_QUEUE_NAME = "CadastroEvent";
        private readonly ILoggerFactory _loggerFactory;

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            _loggerFactory = loggerFactory;
            _loggerFactory.AddDebug(); // logs to the debug output window in VS.
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
            services.AddScoped<IClaimsManager, ClaimsManager>();
            //services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.AddMvc();
            services.AddSingleton<IProfileService, ProfileService>();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients(Configuration["CallbackUrl"]))
                .AddInMemoryPersistedGrants()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddScoped<IMediator, NoMediator>();
            services.AddScoped<IRequest<bool>, CadastroCommand>();
            services.AddMediatR(typeof(CadastroCommand).GetTypeInfo().Assembly);

            RegisterRebus(services);
        }

        private void RegisterRebus(IServiceCollection services)
        {
            // Register handlers 
            services.AutoRegisterHandlersFromAssemblyOf<CadastroEventHandler>();

            // Configure and register Rebus
            services.AddRebus(configure => configure
                .Logging(l => l.Use(new MSLoggerFactoryAdapter(_loggerFactory)))
                .Transport(t => t.UseRabbitMq(RMQ_CONNECTION_STRING, INPUT_QUEUE_NAME)))
                .AddTransient<DbContext, ApplicationDbContext>()
                .AutoRegisterHandlersFromAssemblyOf<CadastroEvent>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseRebus(
                async (bus) =>
                {
                    await bus.Subscribe<CadastroEvent>();
                });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
