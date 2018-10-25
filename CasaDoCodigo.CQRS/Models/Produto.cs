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
        [DataMember]
        public string Codigo { get; private set; }
        [Required]
        [DataMember]
        public string Nome { get; private set; }
        [Required]
        [DataMember]
        [DisplayFormat(DataFormatString="{0:C}")]
        public decimal Preco { get; private set; }
        public string UrlImagem { get { return $"/images/produtos/large_{Codigo}.jpg"; } }

        public Produto(string codigo, string nome, decimal preco)
        {
            this.Codigo = codigo;
            this.Nome = nome;
            this.Preco = preco;
        }

        public Produto(int id, string codigo, string nome, decimal preco)
            : this(codigo, nome, preco)
        {
            this.Id = id;
        }
    }
}
