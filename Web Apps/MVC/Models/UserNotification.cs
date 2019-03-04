using System;

namespace CasaDoCodigo.Models
{
    public class UserNotification
    {
        public UserNotification()
        {
        }

        public UserNotification(string usuarioId, string mensagem, DateTime dateCreated, DateTime? dateVisualized)
        {
            UsuarioId = usuarioId;
            Mensagem = mensagem;
            DateCreated = dateCreated;
            DateVisualized = dateVisualized;
        }

        public string UsuarioId { get; set; }
        public string Mensagem { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateVisualized { get; set; }
    }
}
