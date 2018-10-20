using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Catalogo.Queries
{
    public class ProdutoQueries : IProdutoQueries
    {
        private readonly IConfiguration configuration;

        public ProdutoQueries(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<Produto>> GetProdutosAsync()
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var sql = "select Id, Codigo, Nome, Preco from produto";
                return await connection.QueryAsync<Produto>(sql);
            }
        }

        public async Task<Produto> GetProdutoAsync(string codigo)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var sql = "select Id, Codigo, Nome, Preco from produto where Codigo = @codigo";
                return (await connection.QueryAsync<Produto>(sql, new { codigo })).SingleOrDefault();
            }
        }
    }
}
