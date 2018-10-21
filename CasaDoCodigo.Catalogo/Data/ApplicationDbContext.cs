using CasaDoCodigo.Catalogo.Model;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Catalogo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var produto = modelBuilder.Entity<Produto>();
            produto.HasKey(t => t.Id);
            produto.Property("Codigo").HasColumnType("nvarchar(3)");
            produto.Property("Nome").HasColumnType("nvarchar(255)");
            produto.Property("Preco").HasColumnType("decimal(5,2)");
        }
    }
}




