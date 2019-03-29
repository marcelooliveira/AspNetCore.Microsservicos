using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using CasaDoCodigo.Models;

namespace CasaDoCodigo.API.Areas.Identity.Services
{
    public class UsersDAO
    {
        private IConfiguration _configuration;

        public UsersDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<UsuarioInput> Find(string userID)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuration.GetConnectionString("Default")))
            {
                return await conexao.QueryFirstOrDefaultAsync<UsuarioInput>(
                    "SELECT Id as UsuarioId, PasswordHash " +
                    "FROM dbo.AspNetUsers " +
                    "WHERE Id = @UserID", new { UserID = userID });
            }
        }
    }
}
