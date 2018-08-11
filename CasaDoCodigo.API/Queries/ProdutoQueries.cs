using CasaDoCodigo.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.API.Queries
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
            string connectionString = configuration.GetConnectionString("Default");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = "select Id, Codigo, Nome, Preco from produto with (nolock)";
                return await connection.QueryAsync<Produto>(sql);
            }
        }
    }
}
