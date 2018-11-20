using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreExportAttribute : Attribute
    {
         
    }
}