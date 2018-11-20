using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    public class PropertyImportSelector : IImportSelector
    {
        public IEnumerable<ImportMetadata> SelectImport(IExport exportType) => 
            from property in exportType.ImplementType.GetProperties(AopConstants.DefaultBindingFlags)
            let attr = property.GetCustomAttribute<InjectAttribute>()
            where attr != null
            select
                new ImportMetadata(
                    attr.Interface,
                    attr.ContractName,
                    exportType,
                    property.Name,
                    attr.Optional,
                    attr.CreateMetadata());
    }
}