using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Common.CastleProxy.Impl
{
    [PublicAPI]
    public class MetadataFactory : IMetadataFactory
    {
        private static DictionaryAdapterFactory _fac = new DictionaryAdapterFactory();

        public object CreateMetadata(Type interfaceType, IDictionary<string, object> metadata)
        {
            lock (this)
            {
                //return ProxyGeneratorFactory.ProxyGenerator.CreateClassProxy(typeof(MetadataBase), new[] {interfaceType}, ProxyGenerationOptions.Default, new object[] {metadata}, new MetadataInterceptor());
                
                return _fac.GetAdapter(interfaceType, metadata);
            }
        }
    }
}