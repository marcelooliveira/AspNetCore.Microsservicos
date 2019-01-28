using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogo.API.Queries
{
    public class ProdutoQueries : IProdutoQueries
    {
        private readonly IConfiguration configuration;

        public ProdutoQueries(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<Produto>> GetProdutosAsync(string pesquisa = null)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var sql =
                    "select p.Id, p.Codigo, p.Nome, p.Preco," +
                    "   c.Id as CategoriaId, c.Nome as CategoriaNome" +
                    " from produto as p " +
                    " inner join categoria as c " +
                    "   on c.Id = p.CategoriaId";
                if (string.IsNullOrWhiteSpace(pesquisa))
                {
                    return await connection.QueryAsync<Produto>(sql);
                }
                sql += " where p.nome like @pesquisa or c.nome like @pesquisa";
                return await connection.QueryAsync<Produto>(sql, new { pesquisa = "%" + pesquisa + "%" });
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
