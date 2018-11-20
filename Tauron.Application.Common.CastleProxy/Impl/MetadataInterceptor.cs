//using System.Diagnostics;
//using Castle.DynamicProxy;

//namespace Tauron.Application.Common.CastleProxy.Impl
//{
//    [DebuggerNonUserCode]
//    public sealed class MetadataInterceptor : IInterceptor
//    {
//        public void Intercept(IInvocation invocation)
//        {
//            var name = invocation.Method.Name.Remove(0, 4);
//            var metadata = (MetadataBase)invocation.Proxy;

//            metadata.Metadata.TryGetValue(name, out var value);
//            invocation.ReturnValue = value;
//        }
//    }
//}