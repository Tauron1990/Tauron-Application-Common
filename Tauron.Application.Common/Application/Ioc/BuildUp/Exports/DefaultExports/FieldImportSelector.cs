using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    public sealed class FieldImportSelector : IImportSelector
    {
        public IEnumerable<ImportMetadata> SelectImport(IExport exportType) 
            => from fieldInfo in exportType.ImplementType.GetFields(AopConstants.DefaultBindingFlags)
            let attr = fieldInfo.GetCustomAttribute<InjectAttribute>()
            where attr != null
            select
                new ImportMetadata(
                    attr.Interface,
                    attr.ContractName,
                    exportType,
                    fieldInfo.Name,
                    attr.Optional,
                    attr.CreateMetadata());
    }
}