using Newtonsoft.Json.Serialization;
using System;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CasaDoCodigo.API.Queries;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using CasaDoCodigo.API.Areas.Identity.Data;
using CasaDoCodigo.API.Areas.Identity.Services;
using CasaDoCodigo.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.HealthChecks;
using System.Threading.Tasks;

namespace CasaDoCodigo.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public TokenConfigurations TokenConfigurations { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services
            //    .AddAuthentication()
            //    .AddJwtBearer(bearerOptions =>
            //    {
            //        var paramsValidation = bearerOptions.TokenValidationParameters;
            //        paramsValidation.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]));
            //        paramsValidation.ValidAudience = Configuration["Tokens:Issuer"];
            //        paramsValidation.ValidIssuer = Configuration["Tokens:Issuer"];

            //        // Valida a assinatura de um token recebido
            //        paramsValidation.ValidateIssuerSigningKey = true;

            //        // Verifica se um token recebido ainda é válido
            //        paramsValidation.ValidateLifetime = true;

            //        // Tempo de tolerância para a expiração de um token (utilizado
            //        // caso haja problemas de sincronismo de horário entre diferentes
            //        // computadores envolvidos no processo de comunicação)
            //        paramsValidation.ClockSkew = TimeSpan.Zero;
            //    });

            //services
            //    .AddAuthentication("Bearer")
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.ApiName = "CasaDoCodigo.API";
            //        options.ApiSecret = "secret";
            //        options.Authority = "https://localhost:44338/";
            //    });

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Casa do Código - Web API",
                    Description = "Uma API contendo funcionalidades da aplicação de e-Commerce:" +
                    "catálogo de produtos, cesta de compras, cadastro de cliente e resumo de pedido.",
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

            services.AddScoped<DbContext, ApplicationContext>();
            services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                }
            );

            services.AddTransient<IUserStore<CasaDoCodigoAPIUser>, UserStore<CasaDoCodigoAPIUser>>();
            services.AddTransient<IPasswordHasher<CasaDoCodigoAPIUser>, PasswordHasher<CasaDoCodigoAPIUser>>();
            services.AddTransient<UserManager<CasaDoCodigoAPIUser>>();

            services.AddTransient<IProdutoQueries, ProdutoQueries>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<ICadastroRepository, CadastroRepository>();
            services.AddTransient<IItemPedidoRepository, ItemPedidoRepository>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<UsersDAO>();
            services.AddSingleton<SigningConfigurations>();
            services.AddSingleton<TokenConfigurations>();

            services.AddHealthChecks(checks =>
            {
                checks.AddSqlCheck("CasaDoCodigo", connectionString);
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });
        }

        static private SymmetricSecurityKey GetSignInKey()
        {
            const string secretKey = "very_long_very_secret_secret";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            return signingKey;
        }

        static private string GetIssuer()
        {
            return "issuer";
        }

        static private string GetAudience()
        {
            return "audience";
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Casa do Código - Web API v1");
            });

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();

            serviceProvider.GetService<IDataService>().InicializaDB().Wait();
        }
    }
}
