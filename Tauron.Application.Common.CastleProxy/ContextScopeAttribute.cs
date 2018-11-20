using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.CastleProxy
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ContextScopeAttribute : ExportMetadataBaseAttribute
    {
        public string Name => InternalValue as string;
        
        public ContextScopeAttribute([NotNull] string name)
            : base(AopConstants.ContextMetadataName, name)
        {
            Argument.NotNull(name, nameof(name));
        }

        public ContextScopeAttribute()
            : base(AopConstants.ContextMetadataName, null)
        {
        }
    }
}