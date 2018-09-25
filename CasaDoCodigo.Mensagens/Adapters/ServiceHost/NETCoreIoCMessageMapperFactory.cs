using Paramore.Brighter;
using System;

namespace CasaDoCodigo.Mensagens.Adapters.ServiceHost
{
    public class NETCoreIoCMessageMapperFactory : IAmAMessageMapperFactory
    {
        private IServiceProvider _container;

        public IServiceProvider Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public IAmAMessageMapper Create(Type messageMapperType)
        {
            return (IAmAMessageMapper)Container.GetService(messageMapperType);
        }
    }
}
