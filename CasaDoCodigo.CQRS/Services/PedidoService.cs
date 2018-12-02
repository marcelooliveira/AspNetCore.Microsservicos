using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CasaDoCodigo;
using CasaDoCodigo.OrdemDeCompra.Models.DTOs;
using CasaDoCodigo.Services;
using Microsoft.Extensions.Configuration;

namespace MVC.Services
{
    public class PedidoService : BaseHttpService, IPedidoService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ISessionHelper sessionHelper;

        public PedidoService(IConfiguration configuration
            , HttpClient httpClient
            , ISessionHelper sessionHelper) 
            : base(configuration, httpClient, sessionHelper)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.sessionHelper = sessionHelper;
        }

        class Uris
        {
            public static string GetPedidos => "api/pedido";
        }

        public async Task<List<PedidoDTO>> GetAsync(string clienteId)
        {
            return await GetAsync<List<PedidoDTO>>(Uris.GetPedidos, clienteId);
        }

        public override string Scope => "OrdemDeCompra.API";
    }
}
