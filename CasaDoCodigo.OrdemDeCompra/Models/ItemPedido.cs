using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CasaDoCodigo.OrdemDeCompra.Models
{
    [DataContract]
    public class ItemPedido : BaseModel
    {
        [Required]
        [DataMember]
        public Pedido Pedido { get; set; }
        [DataMember]
        public int ProdutoId { get; set; }
        [Required]
        [DataMember]
        public string ProdutoCodigo { get; set; }
        [Required]
        [DataMember]
        public string ProdutoNome { get; set; }
        [Required]
        [DataMember]
        public decimal ProdutoPreco { get; set; }
        [Required]
        [DataMember]
        public int ProdutoQuantidade { get; set; }
        [DataMember]
        public decimal ProdutoPrecoUnitario { get; set; }
        public decimal Subtotal => ProdutoQuantidade * ProdutoPrecoUnitario;

        public ItemPedido(int produtoId, string produtoCodigo, string produtoNome, decimal produtoPreco, int produtoQuantidade, decimal produtoPrecoUnitario)
        {
            ProdutoId = produtoId;
            ProdutoCodigo = produtoCodigo;
            ProdutoNome = produtoNome;
            ProdutoPreco = produtoPreco;
            ProdutoQuantidade = produtoQuantidade;
            ProdutoPrecoUnitario = produtoPrecoUnitario;
        }

        public void AtualizaQuantidade(int produtoQuantidade)
        {
            ProdutoQuantidade = produtoQuantidade;
        }
    }
}
