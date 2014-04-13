using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Models;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public class PropertyModelExtension : IContainerExtension
    {
        public const string EnablePropertyInheritanceMetadataName = "EnablePropertyInheritance";

        private class PropertyImportInterceptor : IImportInterceptor
        {
            private readonly ImportMetadata[] _metadatas;

            public PropertyImportInterceptor([NotNull] ImportMetadata[] metadatas)
            {
                _metadatas = metadatas;
            }

            public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
            {
                if (!_metadatas.Contains(metadata)) return true;

                var viewModel = (ViewModelBase) target;
                var model = value as ModelBase;

                if (model == null) return true;

                viewModel.RegisterInheritedModel(metadata.ContractName, model);

                return true;
            }
        }

        private class InternalProxyService : IProxyService
        {
            private ModuleScope _moduleScope;
            private ProxyGenerator _genericGenerator;

            public InternalProxyService()
            {
                _moduleScope = new ModuleScope();
                _genericGenerator = new ProxyGenerator(new DefaultProxyBuilder(_moduleScope));
            }

            public ProxyGenerator Generate(ExportMetadata metadata, ImportMetadata[] imports, out IImportInterceptor interceptor)
            {
                interceptor = null;
                if (!typeof (ModelBase).IsAssignableFrom(metadata.Export.ImplementType)) return _genericGenerator;

                var targetImports =
                    imports.Where(meta => meta.Metadata.ContainsKey(EnablePropertyInheritanceMetadataName))
                        .Where(m => (bool) m.Metadata[EnablePropertyInheritanceMetadataName])
                        .ToArray();

                if (targetImports.Length == 0) return _genericGenerator;
            
                interceptor = new PropertyImportInterceptor(targetImports);

                return _genericGenerator;
            }

            public ProxyGenerator GenericGenerator
            {
                get
                {
                    return _genericGenerator;
                }
            }
        }

        [NotNull]
        private static string BuildImportName([NotNull] ImportMetadata metadata)
        {
            return metadata.InterfaceType + metadata.ContractName;
        }

        public void Initialize(ComponentRegistry components)
        {
            components.Register<IProxyService, InternalProxyService>(true);
        }
    }
}