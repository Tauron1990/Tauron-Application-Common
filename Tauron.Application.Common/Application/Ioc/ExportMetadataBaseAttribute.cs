using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [PublicAPI]
    public abstract class ExportMetadataBaseAttribute : Attribute
    {
        protected ExportMetadataBaseAttribute([NotNull] string key, object value)
        {
            Argument.NotNull(key, nameof(key));
            InternalKey = key;
            InternalValue = value;
        }
        protected internal string InternalKey { get; }

        protected internal object InternalValue { get; set; }
    }
}