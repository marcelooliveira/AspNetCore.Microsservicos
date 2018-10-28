using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Infrastructure
{
    public static class UrlAPIs
    {
        public static class Carrinho
        {
            public static string UpdateItemCarrinho(string baseUri) => $"{baseUri}/carrinho/itens";
        }
    }
}
