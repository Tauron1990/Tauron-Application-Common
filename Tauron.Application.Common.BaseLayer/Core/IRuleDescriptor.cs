using System;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public interface IRuleDescriptor
    {
        Type ParameterType { get; }

        Type ReturnType { get; }
    }
}