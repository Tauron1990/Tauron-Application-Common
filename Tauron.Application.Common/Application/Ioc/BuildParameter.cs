using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    public abstract class BuildParameter
    {
        [CanBeNull]
        protected internal abstract IExport CreateExport();
    }
}
