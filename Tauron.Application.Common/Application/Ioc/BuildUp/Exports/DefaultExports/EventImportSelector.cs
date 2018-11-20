using System.Collections.Generic;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    public class EventImportSelector : IImportSelector
    {
        public IEnumerable<ImportMetadata> SelectImport(IExport exportType)
        {
            foreach (var eventInfo in exportType.ImplementType.GetEvents(AopConstants.DefaultBindingFlags))
            {
                var attr = eventInfo.GetCustomAttribute<InjectEventAttribute>();
                if (attr != null)
                {
                    yield return
                        new ImportMetadata(
                            eventInfo.EventHandlerType,
                            attr.Topic,
                            exportType,
                            eventInfo.Name,
                            true,
                            new Dictionary<string, object>());
                }
            }

            foreach (var methodInfo in exportType.ImplementType.GetMethods(AopConstants.DefaultBindingFlags))
            {
                var attr = methodInfo.GetCustomAttribute<InjectEventAttribute>();
                if (attr == null) continue;

                yield return
                    new ImportMetadata(
                        methodInfo.GetType(),
                        attr.Topic,
                        exportType,
                        methodInfo.Name,
                        true,
                        new Dictionary<string, object>
                        {
                            {AopConstants.EventMetodMetadataName, true},
                            {AopConstants.EventTopicMetadataName, attr.Topic}
                        });
            }
        }
    }
}