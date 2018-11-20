using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;


namespace Tauron.Application.Ioc.Components
{
    [PublicAPI]
    public class RebuildManager
    {
        private readonly GroupDictionary<ExportMetadata, BuildObject> _objects =
            new GroupDictionary<ExportMetadata, BuildObject>(typeof(WeakReferenceCollection<BuildObject>));

        public void AddBuild([NotNull] BuildObject instance)
        {
            Argument.NotNull(instance, nameof(instance));

            lock (this)
                _objects[instance.Metadata].Add(instance);
        }

        public IEnumerable<BuildObject> GetAffectedParts(
            [NotNull] IEnumerable<ExportMetadata> added,
            [NotNull] IEnumerable<ExportMetadata> removed)
        {
            Argument.NotNull(added, nameof(added));
            Argument.NotNull(removed, nameof(removed));

            lock (this)
            {
                var changed = added.Concat(removed);

                return from o in _objects
                    from buildObject in o.Value
                    where
                        buildObject.GetImports()
                            .Any(
                                tup =>
                                    changed.Any(
                                        meta =>
                                            tup.InterfaceType == meta.InterfaceType
                                            && tup.ContractName == meta.ContractName))
                    where buildObject.IsAlive
                    select buildObject;
            }
        }
    }
}