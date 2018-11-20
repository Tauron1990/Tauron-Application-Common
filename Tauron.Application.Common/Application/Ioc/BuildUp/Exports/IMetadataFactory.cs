using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    public interface IMetadataFactory
    {
        object CreateMetadata(Type interfaceType, IDictionary<string, object> metadata);
    }
}