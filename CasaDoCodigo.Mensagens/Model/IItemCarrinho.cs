namespace CasaDoCodigo.Mensagens.Model
{
    public interface IItemCarrinho
    {
        string Id { get; set; }
        decimal PrecoUnitario { get; set; }
        string ProdutoId { get; set; }
        string ProdutoNome { get; set; }
        int Quantidade { get; set; }
        decimal Subtotal { get; }
        string UrlImagem { get; }
    }
}