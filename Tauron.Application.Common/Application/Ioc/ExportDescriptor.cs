using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public class ExportDescriptor
    {
        [NotNull]
        public ExportMetadata Meta { get; set; }

        public ExportDescriptor([NotNull] ExportMetadata meta)
        {
            Meta = meta;
        }
    }
}