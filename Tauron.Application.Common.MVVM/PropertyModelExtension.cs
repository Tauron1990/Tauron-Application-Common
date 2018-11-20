using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Models;

namespace Tauron.Application
{
    [Serializable]
    public class PropertyModelExtension : IContainerExtension
    {
        public const string EnablePropertyInheritanceMetadataName = "EnablePropertyInheritance";

        public void Initialize(ComponentRegistry components)
        {
            components.Register<IImportInterceptionService, InternalProxyService>();
        }

        [NotNull]
        private static string BuildImportName([NotNull] ImportMetadata metadata) => metadata.InterfaceType + metadata.ContractName;

        private class PropertyImportInterceptor : IImportInterceptor
        {
            private readonly ImportMetadata[] _metadatas;

            public PropertyImportInterceptor([NotNull] ImportMetadata[] metadatas) => _metadatas = metadatas;

            public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
            {
                if (!_metadatas.Contains(metadata)) return true;

                var viewModel = (ViewModelBase) target;

                if (!(value is ModelBase model)) return true;

                viewModel.RegisterInheritedModel(metadata.ContractName, model);

                return true;
            }
        }

        private class InternalProxyService : IImportInterceptionService
        {
            public IImportInterceptor Get(ExportMetadata metadata, ImportMetadata[] imports)
            {
                if (!typeof(ModelBase).IsAssignableFrom(metadata.Export.ImplementType)) return null;

                var targetImports =
                    imports.Where(meta => meta.Metadata.ContainsKey(EnablePropertyInheritanceMetadataName))
                        .Where(m => (bool)m.Metadata[EnablePropertyInheritanceMetadataName])
                        .ToArray();

                if (targetImports.Length == 0) return null;

                return new PropertyImportInterceptor(targetImports);
            }
        }
    }
}