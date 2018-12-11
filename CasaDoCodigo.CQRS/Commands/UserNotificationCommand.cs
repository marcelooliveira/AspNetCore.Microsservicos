using MediatR;
using System;

namespace MVC.Commands
{
    public class UserNotificationCommand: IRequest<bool>
    {
        public UserNotificationCommand()
        {
        }

        public UserNotificationCommand(string usuarioId, string mensagem, DateTime dateCreated)
        {
            UsuarioId = usuarioId;
            Mensagem = mensagem;
            DateCreated = dateCreated;
        }

        public string UsuarioId { get; set; }
        public string Mensagem { get; set; }
        public DateTime DateCreated { get; set; }
    }
}