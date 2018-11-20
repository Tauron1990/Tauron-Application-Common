using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports; 

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    [Serializable]
    public abstract class ExportProvider
    {
        public event EventHandler<ExportChangedEventArgs> ExportsChanged;

        public abstract IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory);

        protected virtual void OnExportsChanged([NotNull] ExportChangedEventArgs e)
        {
           Argument.NotNull(e, nameof(e));

            var handler = ExportsChanged;
            handler?.Invoke(this, e);
        }

        public virtual bool BroadcastChanges => false;

        public abstract string Technology { get; }
    }

    [PublicAPI]
    [Serializable]
    public class ExportChangedEventArgs : EventArgs
    {
        public ExportChangedEventArgs(
            [NotNull] IEnumerable<ExportMetadata> addedExport,
            [NotNull] IEnumerable<ExportMetadata> removedExport)
        {
            Argument.NotNull(addedExport, nameof(addedExport));
            Argument.NotNull(removedExport, nameof(removedExport));

            Added = addedExport;
            Removed = removedExport;
        }

        public IEnumerable<ExportMetadata> Added { get; }

        public IEnumerable<ExportMetadata> Removed { get; }
    }
}