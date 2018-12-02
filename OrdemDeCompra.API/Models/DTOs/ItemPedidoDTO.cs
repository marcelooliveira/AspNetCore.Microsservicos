namespace CasaDoCodigo.OrdemDeCompra.Models.DTOs
{
    public class ItemPedidoDTO
    {
        public ItemPedidoDTO()
        {

        }

        public ItemPedidoDTO(string produtoCodigo, string produtoNome, int produtoQuantidade, decimal produtoPrecoUnitario)
        {
            ProdutoCodigo = produtoCodigo;
            ProdutoNome = produtoNome;
            ProdutoQuantidade = produtoQuantidade;
            ProdutoPrecoUnitario = produtoPrecoUnitario;
        }

        public string ProdutoCodigo { get; set; }
        public string ProdutoNome { get; set; }
        public int ProdutoQuantidade { get; set; }
        public decimal ProdutoPrecoUnitario { get; set; }
        public decimal Subtotal => ProdutoQuantidade * ProdutoPrecoUnitario;
    }
}
