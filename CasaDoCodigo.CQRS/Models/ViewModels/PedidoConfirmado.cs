using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models.ViewModels
{
    public class PedidoConfirmado
    {
        public PedidoConfirmado()
        {

        }

        public PedidoConfirmado(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}
