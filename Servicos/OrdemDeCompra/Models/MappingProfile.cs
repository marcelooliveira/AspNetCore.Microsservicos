using AutoMapper;
using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdemDeCompra.API.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pedido, PedidoDTO>();
            CreateMap<ItemPedido, ItemPedidoDTO>();
        }
    }
}
