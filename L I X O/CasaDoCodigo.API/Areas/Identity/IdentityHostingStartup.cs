using System;
using CasaDoCodigo.API.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CasaDoCodigo.API.Areas.Identity.IdentityHostingStartup))]
namespace CasaDoCodigo.API.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ApplicationContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Default")));

                services.AddDefaultIdentity<CasaDoCodigoAPIUser>()
                    .AddEntityFrameworkStores<ApplicationContext>();
            });
        }
    }
}