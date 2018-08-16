using System.Collections.Generic;
using System.Threading.Tasks;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Mvc;
using CasaDoCodigo.API.Queries;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : BaseApiController
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IProdutoQueries produtoQueries;

        public ProdutoController(ILogger<CarrinhoController> logger,
            IProdutoRepository produtoRepository, IProdutoQueries produtoQueries) : base(logger)
        {
            this.produtoRepository = produtoRepository;
            this.produtoQueries = produtoQueries;
        }

        /// <summary>
        /// Obtém a lista completa de produtos do catálogo.
        /// </summary>
        /// <returns>
        /// A lista completa de produtos do catálogo
        /// </returns>
        [HttpGet]
        public async Task<IEnumerable<Queries.Produto>> GetProdutos()
        {
            return await produtoQueries.GetProdutosAsync();
        }
    }
}
