using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

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
            services.AddTransient<IPedidoService, PedidoService>();
            services.AddTransient<ICatalogoService, CatalogoService>();
            services.AddTransient<ICarrinhoService, CarrinhoService>();
            services.AddTransient<ISessionHelper, SessionHelper>();
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
            services.AddMvc()
                .AddJsonOptions(a => a.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddDistributedMemoryCache();
            services.AddSession();
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
                    template: "{controller=Catalogo}/{action=Index}/{codigo?}");
            });
        }
    }
}
