using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [PublicAPI]
    public sealed class ExportMetadataAttribute : ExportMetadataBaseAttribute
    {
        public ExportMetadataAttribute(string key, object value)
            : base(key, value){}

        public string Key => InternalKey;

        public object Value => InternalValue;
    }
}