using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models.ViewModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Telefone { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        public string Complemento { get; set; }
        [Required]
        public string Bairro { get; set; }
        [Required]
        public string Municipio { get; set; }
        [Required]
        public string UF { get; set; }
        [Required]
        public string CEP { get; set; }
    }
}
