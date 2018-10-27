//using System;

//namespace CasaDoCodigo.Mensagens.Adapters.ServiceHost
//{
//    public class NETCoreIoCHandlerFactory : IAmAHandlerFactory
//    {
//        public IServiceProvider Container { get; set; }

//        public IHandleRequests Create(Type handlerType)
//        {
//            return (IHandleRequests)Container.GetService(handlerType);
//        }

//        public void Release(IHandleRequests handler)
//        {
//            var disposable = handler as IDisposable;
//            if (disposable != null)
//            {
//                disposable.Dispose();
//            }
//            handler = null;
//        }
//    }
//}
