using System.ComponentModel.DataAnnotations;

namespace Catalogo.API.Model
{
    public class Categoria : BaseModel
    {
        public Categoria() { }

        public Categoria(string nome)
        {
            Nome = nome;
        }

        [Required]
        public string Nome { get; private set; }
    }
}
