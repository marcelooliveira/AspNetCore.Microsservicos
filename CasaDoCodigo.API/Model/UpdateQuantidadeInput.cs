using CasaDoCodigo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models
{
    public class UpdateQuantidadeInput
    {
        public UpdateQuantidadeInput(int itemPedidoId, int quantidade)
        {
            ItemPedidoId = itemPedidoId;
            Quantidade = quantidade;
        }

        public int ItemPedidoId { get; set; }
        public int Quantidade { get; set; }
    }
}
