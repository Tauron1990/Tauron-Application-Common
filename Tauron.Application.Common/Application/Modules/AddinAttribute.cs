using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [AttributeUsage(AttributeTargets.Class), PublicAPI]
    public sealed class AddinAttribute : ExportModuleAttribute
    {

    }
}
