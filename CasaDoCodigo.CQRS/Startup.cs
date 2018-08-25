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

namespace CasaDoCodigo
{
    public class Startup
    {
        private const string API_BASE_URI = "https://localhost:44339";

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
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IApiService, ApiService>()
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
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
    }

}
