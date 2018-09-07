using CasaDoCodigo.Carrinho.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Carrinho.IntegrationEvents
{
    public class CheckoutIntegrationEvent : IntegrationEvent
    {
        public CheckoutIntegrationEvent(CarrinhoCliente carrinhoCliente)
        {
            CarrinhoCliente = carrinhoCliente;
        }

        public CarrinhoCliente CarrinhoCliente { get; set; }
    }
}
