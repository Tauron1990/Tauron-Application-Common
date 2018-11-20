using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.CastleProxy.Impl
{
    internal class InternalInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var name = method.Name;
            // ReSharper disable once InvertIf
            if (method.IsSpecialName)
            {
                if (name.StartsWith(AopConstants.PropertyGetter, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.PropertyGetter.Length);

                if (name.StartsWith(AopConstants.PropertySetter, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.PropertySetter.Length);

                if (name.StartsWith(AopConstants.EventAdder, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.EventAdder.Length);

                if (name.StartsWith(AopConstants.EventRemover, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.EventRemover.Length);
            }

            return interceptors.Where(
                    inter =>
                    {
                        if (inter is ISpecificInterceptor sinter)
                        {
                            return sinter.Name == name ||
                                   sinter.Name == AopConstants.InternalUniversalInterceptorName;
                        }

                        return true;
                    })
                .OrderBy(inter => !(inter is ISpecificInterceptor sinter) ? 0 : sinter.Order)
                .ToArray();
        }
    }
}