using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;

namespace Tauron.Application.Common.MVVM.Dynamic.Impl
{
    public class TypeSelector : IImportSelector
    {
        private readonly InternalAssemblyBuilder _internalAssemblyBuilder;

        public TypeSelector(InternalAssemblyBuilder internalAssemblyBuilder) => _internalAssemblyBuilder = internalAssemblyBuilder;

        public IEnumerable<ImportMetadata> SelectImport(IExport exportType)
        {
            if(exportType.ImplementType.IsAbstract && exportType.ImplementType.HasAttribute<CreateRuleCallAttribute>())
                _internalAssemblyBuilder.AddType(exportType);

            return Enumerable.Empty<ImportMetadata>();
        }
    }
}