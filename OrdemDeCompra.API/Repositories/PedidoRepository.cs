using CasaDoCodigo.OrdemDeCompra;
using CasaDoCodigo.OrdemDeCompra.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;

namespace CasaDoCodigo.OrdemDeCompra.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        public PedidoRepository(DbContext contexto) : base(contexto)
        {
        }

        public async Task<Pedido> CreateOrUpdate(Pedido pedido)
        {
            if (pedido == null)
                throw new ArgumentNullException();

            if (pedido.Itens.Count == 0)
                throw new NoItemsException();

            foreach (var item in pedido.Itens)
            {
                if (
                    string.IsNullOrWhiteSpace(item.ProdutoCodigo)
                    || string.IsNullOrWhiteSpace(item.ProdutoNome)
                    || item.ProdutoQuantidade <= 0
                    || item.ProdutoPrecoUnitario <= 0
                    )
                {
                    throw new InvalidItemException();
                }
            }

            if (string.IsNullOrWhiteSpace(pedido.ClienteId)
                 || string.IsNullOrWhiteSpace(pedido.ClienteNome)
                 || string.IsNullOrWhiteSpace(pedido.ClienteEmail)
                 || string.IsNullOrWhiteSpace(pedido.ClienteTelefone)
                 || string.IsNullOrWhiteSpace(pedido.ClienteEndereco)
                 || string.IsNullOrWhiteSpace(pedido.ClienteComplemento)
                 || string.IsNullOrWhiteSpace(pedido.ClienteBairro)
                 || string.IsNullOrWhiteSpace(pedido.ClienteMunicipio)
                 || string.IsNullOrWhiteSpace(pedido.ClienteUF)
                 || string.IsNullOrWhiteSpace(pedido.ClienteCEP)
                )
                throw new InvalidUserDataException();

            EntityEntry<Pedido> entityEntry;
            try
            {
                entityEntry = await dbSet.AddAsync(pedido);
                await contexto.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return entityEntry.Entity;
        }
    }


    [Serializable]
    public class NoItemsException : Exception
    {
        public NoItemsException() {}
        public NoItemsException(string message) : base(message) { }
        public NoItemsException(string message, Exception inner) : base(message, inner) { }
        protected NoItemsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InvalidItemException : Exception
    {
        public InvalidItemException() { }
        public InvalidItemException(string message) : base(message) { }
        public InvalidItemException(string message, Exception inner) : base(message, inner) { }
        protected InvalidItemException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InvalidUserDataException : Exception
    {
        public InvalidUserDataException() { }
        public InvalidUserDataException(string message) : base(message) { }
        public InvalidUserDataException(string message, Exception inner) : base(message, inner) { }
        protected InvalidUserDataException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
