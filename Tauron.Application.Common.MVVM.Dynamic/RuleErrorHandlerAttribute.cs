using System;
using JetBrains.Annotations;

namespace Tauron.Application.Common.MVVM.Dynamic
{
    [AttributeUsage(AttributeTargets.Method), MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class RuleErrorHandlerAttribute : Attribute
    {
        
    }
}