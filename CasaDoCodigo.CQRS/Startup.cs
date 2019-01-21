using CasaDoCodigo.Controllers;
using CasaDoCodigo.Mensagens.IntegrationEvents.Events;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MVC;
using MVC.Commands;
using MVC.Model.Redis;
using MVC.SignalR;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Rebus.Config;
using Rebus.ServiceProvider;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CasaDoCodigo
{
    public class Startup
    {
        //private const string RMQ_CONNECTION_STRING = "amqp://localhost";
        private const string RMQ_CONNECTION_STRING = "amqp://rabbitmq";
        private const string INPUT_QUEUE_NAME = "UserNotificationEvent";
        private readonly ILoggerFactory _loggerFactory;

        public Startup(ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
            _loggerFactory.AddDebug(); // logs to the debug output window in VS.
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var uri = new Uri(Configuration["ApiUrl"]);
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = uri
            };

            services.AddSingleton(typeof(HttpClient), httpClient);
            services.AddHttpContextAccessor();
            services.AddTransient<IPedidoService, PedidoService>();
            services.AddTransient<ICatalogoService, CatalogoService>();
            services.AddTransient<ICarrinhoService, CarrinhoService>();
            services.AddTransient<ISessionHelper, SessionHelper>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            services.AddMvc()
                .AddJsonOptions(a => a.SerializerSettings.ContractResolver = new DefaultContractResolver());
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
                //var settings = sp.GetRequiredService<IOptions<RedisConfig>>().Value;
                //var configuration = ConfigurationOptions.Parse("localhost", true);
                var configuration = ConfigurationOptions.Parse("ecommerce.data:6379", true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddAuthorization();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";

                    options.Authority = Configuration["IdentityUrl"];
                    //options.BackchannelHttpHandler = new HttpClientHandler() { Proxy = new WebProxy(Configuration["System:Proxy"]) };
                    options.BackchannelHttpHandler = new HttpClientHandler() { Proxy = new WebProxy() };
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "MVC";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnUserInformationReceived = (context) =>
                        {
                            if (!(context.Principal.Identity is ClaimsIdentity claimsId))
                            {
                                throw new Exception();
                            }

                            // Get a list of all claims attached to the UserInformationRecieved context
                            var ctxClaims = context.User.Children().ToList();

                            foreach (var ctxClaim in ctxClaims)
                            {
                                var claimType = ctxClaim.Path;
                                var token = ctxClaim.FirstOrDefault();
                                if (token == null)
                                {
                                    continue;
                                }

                                var claims = new List<Claim>();
                                if (token.Children().Any())
                                {
                                    claims.AddRange(
                                        token.Children()
                                            .Select(c => new Claim(claimType, c.Value<string>())));
                                }
                                else
                                {
                                    claims.Add(new Claim(claimType, token.Value<string>()));
                                }

                                foreach (var claim in claims)
                                {
                                    if (!claimsId.Claims.Any(
                                        c => c.Type == claim.Type &&
                                             c.Value == claim.Value))
                                    {
                                        claimsId.AddClaim(claim);
                                    }
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.Scope.Add("Carrinho.API");
                    options.Scope.Add("OrdemDeCompra.API");
                    options.Scope.Add("CasaDoCodigo.API");
                    options.Scope.Add("offline_access");
                });
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            services.AddTransient<IUserRedisRepository, UserRedisRepository>();
            services.AddMediatR(typeof(UserNotificationCommand).GetTypeInfo().Assembly);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalogo}/{action=Index}/{codigo?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<UserNotificationHub>("/usernotificationhub",
                    options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                    });
            });
        }
    }
}
