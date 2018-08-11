using CasaDoCodigo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface ICadastroRepository
    {
        Task<Cadastro> Update(int cadastroId, Cadastro novoCadastro);
    }

    public class CadastroRepository : BaseRepository<Cadastro>, ICadastroRepository
    {
        public CadastroRepository(ApplicationContext contexto) : base(contexto)
        {
        }

        public async Task<Cadastro> Update(int cadastroId, Cadastro novoCadastro)
        {
            var cadastroDB = 
                await dbSet.Where(c => c.Id == cadastroId)
                .SingleOrDefaultAsync();

            if (cadastroDB == null)
            {
                throw new ArgumentNullException("cadastro");
            }

            cadastroDB.Update(novoCadastro);
            await contexto.SaveChangesAsync();
            return cadastroDB;
        }
    }
}
