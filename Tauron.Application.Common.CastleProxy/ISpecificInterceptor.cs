using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Tauron.Application.Common.CastleProxy
{
    [PublicAPI]
    public interface ISpecificInterceptor : IInterceptor
    {
        string Name { get; }
        
        int Order { get; }
    }
}