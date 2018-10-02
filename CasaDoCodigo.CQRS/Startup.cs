using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CasaDoCodigo.Infrastructure;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.HealthChecks;

namespace CasaDoCodigo
{
    public class Startup
    {
        private const string API_BASE_URI = "https://localhost:44320";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();

            string connectionString = Configuration.GetConnectionString("Default");

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            var uri = new Uri(API_BASE_URI);
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = uri
            };

            services.AddSingleton(typeof(HttpClient), httpClient);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<ICarrinhoService, CarrinhoService>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IApiService, ApiService>()
               .SetHandlerLifetime(TimeSpan.FromMinutes(5))
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
               .AddPolicyHandler(GetRetryPolicy())
               .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHealthChecks(checks =>
            {
                checks.AddUrlCheck(Configuration["ApiUrlHC"]);
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });
        }

        // Este método é chamado pelo runtime.
        // Use este método para configurar o pipeline de requisições HTTP.
        ///<image url="$(ItemDir)\pipeline.png"/>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
            });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }

}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using CasaDoCodigo.Infrastructure;
//using CasaDoCodigo.Services;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Polly;
//using Polly.Extensions.Http;
//using Microsoft.Extensions.HealthChecks;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.OpenIdConnect;

//namespace CasaDoCodigo
//{
//    public class Startup
//    {

//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services
//                .AddCustomMvc(Configuration)
//                .AddHttpClientServices()
//                .AddHealthChecks(Configuration);
//                //.AddCustomAuthentication(Configuration);
//        }


//        // Este método é chamado pelo runtime.
//        // Use este método para configurar o pipeline de requisições HTTP.
//        ///<image url="$(ItemDir)\pipeline.png"/>
//        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
//            IServiceProvider serviceProvider)
//        {

//            if (env.IsDevelopment())
//            {
//                app.UseBrowserLink();
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseExceptionHandler("/Home/Error");
//            }

//            app.UseStaticFiles();
//            app.UseSession();
//            app.UseMvc(routes =>
//            {
//                routes.MapRoute(
//                    name: "default",
//                    template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
//            });
//        }

//    }

//    static class ServiceCollectionExtensions
//    {
//        private const string API_BASE_URI = "https://localhost:44320";

//        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddMvc();
//            services.AddDistributedMemoryCache();
//            services.AddSession();

//            string connectionString = configuration.GetConnectionString("Default");

//            services.AddDbContext<ApplicationContext>(options =>
//                options.UseSqlServer(connectionString)
//            );

//            return services;
//        }

//        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
//        {
//            var uri = new Uri(API_BASE_URI);
//            HttpClient httpClient = new HttpClient()
//            {
//                BaseAddress = uri
//            };

//            services.AddSingleton(typeof(HttpClient), httpClient);
//            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//            services.AddTransient<IApiService, ApiService>();
//            services.AddTransient<ICarrinhoService, CarrinhoService>();
//            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

//            services.AddHttpClient<IApiService, ApiService>()
//               .SetHandlerLifetime(TimeSpan.FromMinutes(5))
//               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
//               .AddPolicyHandler(GetRetryPolicy())
//               .AddPolicyHandler(GetCircuitBreakerPolicy());

//            return services;
//        }

//        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddHealthChecks(checks =>
//            {
//                checks.AddUrlCheck(configuration["ApiUrlHC"]);
//                checks.AddValueTaskCheck("HTTP Endpoint", () => new
//                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
//            });

//            return services;
//        }

//        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
//        {
//            var useLoadTest = configuration.GetValue<bool>("UseLoadTest");
//            var identityUrl = configuration.GetValue<string>("IdentityUrl");
//            var callBackUrl = configuration.GetValue<string>("CallBackUrl");

//            // Add Authentication services          

//            services.AddAuthentication(options =>
//            {
//                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//            })
//            .AddCookie()
//            .AddOpenIdConnect(options =>
//            {
//                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                options.Authority = identityUrl.ToString();
//                options.SignedOutRedirectUri = callBackUrl.ToString();
//                options.ClientId = useLoadTest ? "mvctest" : "mvc";
//                options.ClientSecret = "secret";
//                options.ResponseType = useLoadTest ? "code id_token token" : "code id_token";
//                options.SaveTokens = true;
//                options.GetClaimsFromUserInfoEndpoint = true;
//                options.RequireHttpsMetadata = false;
//                options.Scope.Add("openid");
//                options.Scope.Add("profile");
//                options.Scope.Add("orders");
//                options.Scope.Add("basket");
//                options.Scope.Add("marketing");
//                options.Scope.Add("locations");
//                options.Scope.Add("webshoppingagg");
//                options.Scope.Add("orders.signalrhub");
//            });

//            return services;
//        }

//        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//        {
//            return HttpPolicyExtensions
//              .HandleTransientHttpError()
//              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
//              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
//        }

//        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
//        {
//            return HttpPolicyExtensions
//                .HandleTransientHttpError()
//                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
//        }
//    }
//}
