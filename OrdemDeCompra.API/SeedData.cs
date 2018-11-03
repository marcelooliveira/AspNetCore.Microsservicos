using CasaDoCodigo.OrdemDeCompra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdemDeCompra.API
{
    public class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationContext>();
                await context.Database.MigrateAsync();
            }
        }
    }
}
