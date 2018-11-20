using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    public class ImportSelectorChain : IImportSelectorChain
    {
        private readonly List<IImportSelector> _selectors = new List<IImportSelector>();

        public void Register(IImportSelector selector) => _selectors.Add(Argument.NotNull(selector, nameof(selector)));

        public IEnumerable<ImportMetadata> SelectImport(IExport exportType)
        {
            Argument.NotNull(exportType, nameof(exportType));

            var metadatas = new HashSet<ImportMetadata>();
            foreach (var importMetadata in
                _selectors.SelectMany(selector => selector.SelectImport(exportType)))
                metadatas.Add(importMetadata);

            return metadatas;
        }
    }
}