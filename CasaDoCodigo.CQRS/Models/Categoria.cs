using System;
using System.ComponentModel.DataAnnotations;

namespace CasaDoCodigo.Models
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

        public override bool Equals(object obj)
        {
            var categoria = obj as Categoria;
            return categoria != null &&
                   Id == categoria.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nome);
        }
    }
}
