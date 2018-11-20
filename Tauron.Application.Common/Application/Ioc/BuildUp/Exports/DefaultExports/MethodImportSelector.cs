using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    public class MethodImportSelector : IImportSelector
    {
        public IEnumerable<ImportMetadata> SelectImport(IExport exportType)
        {
            foreach (var methodInfo in exportType.ImplementType.GetMethods(AopConstants.DefaultBindingFlags))
            {
                var attr = methodInfo.GetCustomAttribute<InjectAttribute>();
                if (attr == null) continue;

                var meta = attr.CreateMetadata();

                if (methodInfo.GetParameters().Length == 1)
                {
                    yield return
                        new ImportMetadata(
                            attr.Interface,
                            attr.ContractName,
                            exportType,
                            methodInfo.Name,
                            attr.Optional,
                            meta);
                }
                else
                {
                    meta.Add(AopConstants.ParameterMetadataName, Helper.MapParameters(methodInfo).ToArray());

                    yield return
                        new ImportMetadata(
                            null,
                            null,
                            exportType,
                            methodInfo.Name,
                            false,
                            meta);
                }
            }
        }
    }
}