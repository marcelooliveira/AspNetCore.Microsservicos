using CasaDoCodigo.API.Areas.Identity.Data;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> GetPedido();
        Task AddItem(int pedidoId, string codigo);
        Task<UpdateQuantidadeOutput> UpdateQuantidade(UpdateQuantidadeInput input);
        Task<Pedido> UpdateCadastro(int pedidoId, Cadastro cadastro);
        Task<Pedido> GetPedido(int pedidoId);
    }

    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IItemPedidoRepository itemPedidoRepository;
        private readonly ICadastroRepository cadastroRepository;

        public PedidoRepository(ApplicationContext contexto,
            IHttpContextAccessor contextAccessor,
            IItemPedidoRepository itemPedidoRepository,
            ICadastroRepository cadastroRepository) : base(contexto)
        {
            this.contextAccessor = contextAccessor;
            this.itemPedidoRepository = itemPedidoRepository;
            this.cadastroRepository = cadastroRepository;
        }

        public async Task AddItem(int pedidoId, string codigo)
        {
            var produto = await contexto.Set<Produto>()
                            .Where(p => p.Codigo == codigo)
                            .SingleOrDefaultAsync();

            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado");
            }

            var pedido = await GetPedido(pedidoId);

            var itemPedido = await contexto.Set<ItemPedido>()
                                .Where(i => i.Produto.Codigo == codigo
                                        && i.Pedido.Id == pedido.Id)
                                .SingleOrDefaultAsync();

            if (itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);

                await contexto.Set<ItemPedido>()
                    .AddAsync(itemPedido);

                await contexto.SaveChangesAsync();
            }
        }

        public async Task<Pedido> GetPedido()
        {
            var pedidoId = GetPedidoId() ?? throw new ArgumentNullException("pedidoId");
            return await GetPedido(pedidoId);
        }

        public async Task<Pedido> GetPedido(int pedidoId)
        {
            var pedido = 
                await dbSet
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(p => p.Cadastro)
                .Where(p => p.Id == pedidoId)
                .SingleOrDefaultAsync();

            if (pedido == null)
            {
                pedido = new Pedido();
                dbSet.Add(pedido);
                await contexto.SaveChangesAsync();
                SetPedidoId(pedido.Id);
            }

            return pedido;
        }

        private int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        private void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }

        public async Task<UpdateQuantidadeOutput> UpdateQuantidade(UpdateQuantidadeInput input)
        {
            var itemPedidoDB = await itemPedidoRepository.GetItemPedido(input.ItemPedidoId);

            if (itemPedidoDB != null)
            {
                itemPedidoDB.AtualizaQuantidade(input.Quantidade);

                if (input.Quantidade == 0)
                {
                    await itemPedidoRepository.RemoveItemPedido(input.ItemPedidoId);
                }

                await contexto.SaveChangesAsync();

                var pedido = await GetPedido();
                var carrinhoViewModel = new CarrinhoViewModel(pedido.Id, pedido.Itens);

                return new UpdateQuantidadeOutput(itemPedidoDB, carrinhoViewModel);
            }

            throw new ArgumentException("ItemPedido não encontrado");
        }

        public async Task<Pedido> UpdateCadastro(int pedidoId, Cadastro cadastro)
        {
            await cadastroRepository.Update(pedidoId, cadastro);
            return await GetPedido(pedidoId);
        }
    }
}
