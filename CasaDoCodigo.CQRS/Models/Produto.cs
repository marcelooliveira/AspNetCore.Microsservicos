using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CasaDoCodigo.Models
{
    public class Produto : BaseModel
    {
        public Produto()
        {

        }

        [Required]
        public Categoria Categoria
        {
            get
            {
                return new Categoria(CategoriaNome) { Id = CategoriaId };
            }
        }
        [Required]
        [DataMember]
        public string Codigo { get; private set; }
        [Required]
        [DataMember]
        public string Nome { get; private set; }
        [Required]
        [DataMember]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Preco { get; private set; }
        public string UrlImagem { get { return $"/images/produtos/large_{Codigo}.jpg"; } }
        [DataMember]
        public int CategoriaId { get; set; }
        [DataMember]
        public string CategoriaNome { get; set; }

        public Produto(string codigo, string nome, decimal preco, int categoriaId, string categoriaNome)
        {
            this.Codigo = codigo;
            this.Nome = nome;
            this.Preco = preco;
            this.CategoriaId = categoriaId;
            this.CategoriaNome = categoriaNome;
        }
    }
}
