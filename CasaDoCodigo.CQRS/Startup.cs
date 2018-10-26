using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace CasaDoCodigo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
            services.AddTransient<ICatalogoService, CatalogoService>();
            services.AddTransient<ICarrinhoService, CarrinhoService>();
            services.AddTransient<ISessionHelper, SessionHelper>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
            services.AddMvc()
                .AddJsonOptions(a => a.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddDistributedMemoryCache();
            services.AddSession();

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
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "CasaDoCodigo.MVC";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("Carrinho.API");
                    options.Scope.Add("CasaDoCodigo.API");
                    options.Scope.Add("offline_access");
                });
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
                    template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
            });
        }
    }
}
