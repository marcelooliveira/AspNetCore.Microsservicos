using Catalogo.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var categoria = modelBuilder.Entity<Categoria>();
            categoria.HasKey(t => t.Id);
            categoria.Property("Nome").HasColumnType("nvarchar(255)");

            var produto = modelBuilder.Entity<Produto>();
            produto.HasKey(t => t.Id);
            produto.Property("Codigo").HasColumnType("nvarchar(3)");
            produto.Property("Nome").HasColumnType("nvarchar(255)");
            produto.Property("Preco").HasColumnType("decimal(5,2)");
        }
    }
}




