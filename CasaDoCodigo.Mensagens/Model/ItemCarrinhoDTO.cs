namespace CasaDoCodigo.Mensagens.Model
{
    public class ItemCarrinhoDTO
    {
        public ItemCarrinhoDTO()
        {

        }

        public ItemCarrinhoDTO(string id, string produtoId, string produtoNome, decimal precoUnitario, int quantidade)
        {
            Id = id;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
        }

        public string Id { get; set; }
        public string ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
        public string UrlImagem { get { return $"/images/produtos/large_{ProdutoId}.jpg"; } }
    }
}
