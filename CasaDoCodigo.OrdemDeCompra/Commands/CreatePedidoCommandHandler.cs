using CasaDoCodigo.OrdemDeCompra.Models;
using CasaDoCodigo.OrdemDeCompra.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Commands
{
    public class CreatePedidoCommandHandler
        : IRequestHandler<IdentifiedCommand<CreatePedidoCommand, bool>, bool>
    {
        private readonly IPedidoRepository pedidoRepository;
        private readonly ILoggerFactory _loggerFactory;

        public CreatePedidoCommandHandler(
            ILoggerFactory loggerFactory
            , IPedidoRepository pedidoRepository)
        {
            this.pedidoRepository = pedidoRepository;
            _loggerFactory = loggerFactory;
            _loggerFactory.AddDebug();
        }

        public async Task<bool> Handle(IdentifiedCommand<CreatePedidoCommand, bool> request, CancellationToken cancellationToken)
        {
            var cmd = request.Command;

            var itens = cmd.Itens.Select(
                    i => new ItemPedido(i.ProdutoId, i.ProdutoCodigo, i.ProdutoNome, i.ProdutoPreco, i.ProdutoQuantidade, i.ProdutoPrecoUnitario)
                ).ToList();

            var pedido = new Models.Pedido(itens, cmd.ClienteNome, cmd.ClienteEmail, cmd.ClienteTelefone, cmd.ClienteEndereco, cmd.ClienteComplemento, cmd.ClienteBairro, cmd.ClienteMunicipio, cmd.ClienteUF, cmd.ClienteCEP);

            try
            {
                await this.pedidoRepository.CreateOrUpdate(pedido);
                return true;
            }
            catch (Exception e)
            {
                //TODO: gerar log aqui
                return false;
            }
        }
    }
}
