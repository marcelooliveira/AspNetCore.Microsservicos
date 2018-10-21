using CasaDoCodigo.Catalogo.Data;
using CasaDoCodigo.Catalogo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Catalogo
{
    internal class SeedData
    {
        internal static async Task EnsureSeedData(IServiceProvider services)
        {
            using (var scope = services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                await SaveLivros(context);
            }
        }

        private static async Task SaveLivros(ApplicationDbContext context)
        {
            var dbSet = context.Set<Produto>();

            var livros = await GetLivros();

            foreach (var livro in livros)
            {
                if (!await dbSet.Where(p => p.Codigo == livro.Codigo).AnyAsync())
                {
                    await dbSet.AddAsync(new Produto(livro.Codigo, livro.Nome, livro.Preco));
                }
            }
            await context.SaveChangesAsync();
        }

        static async Task<List<Livro>> GetLivros()
        {
            var json = await File.ReadAllTextAsync("livros.json");
            return JsonConvert.DeserializeObject<List<Livro>>(json);
        }
    }

    public class Livro
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}