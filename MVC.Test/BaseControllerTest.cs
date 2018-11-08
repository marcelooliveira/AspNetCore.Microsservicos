using CasaDoCodigo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVC.Test
{
    public class BaseControllerTest
    {
        protected IList<Produto> GetFakeProdutos()
        {
            return new List<Produto>
            {
                new Produto("001", "produto 001", 12.34m),
                new Produto("002", "produto 002", 23.45m),
                new Produto("003", "produto 003", 34.56m)
            };
        }
    }
}
