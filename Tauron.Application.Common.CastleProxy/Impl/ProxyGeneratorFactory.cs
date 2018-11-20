using Castle.DynamicProxy;

namespace Tauron.Application.Common.CastleProxy.Impl
{
    public static class ProxyGeneratorFactory
    {
        private static ProxyGenerator _generator;

        public static ProxyGenerator ProxyGenerator => _generator ?? (_generator = new ProxyGenerator());
    }
}