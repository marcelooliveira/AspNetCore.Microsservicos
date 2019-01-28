using Catalogo.API.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.API.Controllers
{
    [Route("api/[controller]")]
    public class BuscaController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IProdutoQueries produtoQueries;

        public BuscaController(ILogger<ProdutoController> logger,
            IProdutoQueries produtoQueries)
        {
            this.logger = logger;
            this.produtoQueries = produtoQueries;
        }

        /// <summary>
        /// Obtém a lista completa de produtos do catálogo.
        /// </summary>
        /// <returns>
        /// A lista completa de produtos do catálogo
        /// </returns>
        /// <param name="pesquisa"></param>
        /// <response code="401">Não autorizado</response> 
        /// <returns></returns>
        [HttpGet("{pesquisa}")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos(string pesquisa)
        {
            IEnumerable<Produto> produtos = await produtoQueries.GetProdutosAsync(pesquisa);
            return Ok(produtos);
        }
    }
}