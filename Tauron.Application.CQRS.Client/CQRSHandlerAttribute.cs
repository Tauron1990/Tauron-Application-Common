using System;
using JetBrains.Annotations;

namespace Tauron.Application.CQRS.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class CQRSHandlerAttribute : Attribute
    {
        
    }
}