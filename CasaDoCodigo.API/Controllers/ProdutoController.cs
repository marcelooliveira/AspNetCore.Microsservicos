using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Queries = CasaDoCodigo.API.Queries;
using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CasaDoCodigo.API.Queries;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IProdutoQueries produtoQueries;

        public ProdutoController(IProdutoRepository produtoRepository, IProdutoQueries produtoQueries)
        {
            this.produtoRepository = produtoRepository;
            this.produtoQueries = produtoQueries;
        }

        // GET: api/Produto
        [HttpGet]
        public async Task<IEnumerable<Queries.Produto>> GetProdutos()
        {
            return await produtoQueries.GetProdutosAsync();
        }
    }
}
