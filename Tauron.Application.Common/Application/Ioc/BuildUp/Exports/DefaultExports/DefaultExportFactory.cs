using System;
using System.Linq;
using System.Reflection;
using ExpressionBuilder;
using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    public sealed class DefaultExportFactory : IExportFactory
    {
        private IImportSelectorChain Chain => _componentRegistry.Get<IImportSelectorChain>();

        private ComponentRegistry _componentRegistry;

        public static readonly DefaultExportFactory Factory = new DefaultExportFactory();
        
        public string TechnologyName => AopConstants.DefaultExportFactoryName;

        private DefaultExportFactory()
        {
        }

        public void Initialize(ComponentRegistry components) => _componentRegistry = components;

        public IExport Create([NotNull] Type type, ref int level)
        {
            if (!DefaultExport.IsExport(Argument.NotNull(type, nameof(type)))) return null;

            var export = new DefaultExport(
                type,
                new ExternalExportInfo(false, false, true, false, null, string.Empty),
                false);

            var attr = type.GetCustomAttribute<ExportLevelAttribute>();
            if (attr != null) level = attr.Level;


            export.ImportMetadata = Chain.SelectImport(export);

            return export;
        }
        
        public IExport CreateAnonymos([NotNull] Type type, object[] args)
        {
            var export = new DefaultExport(
                Argument.NotNull(type, nameof(type)),
                new ExternalExportInfo(
                    true,
                    true,
                    true,
                    true,
                    con => Operation.CreateInstance(type, args.Select(Operation.Constant).ToArray()), //Activator.CreateInstance(type, args),
                    type.Name),
                true);

            export.ImportMetadata = Chain.SelectImport(export);
            return export;
        }
        
        [NotNull]
        public IExport CreateAnonymosWithTarget([NotNull] Type type, [NotNull] object target)
        {
            Argument.NotNull(type, nameof(type));
            Argument.NotNull(target, nameof(target));

            var info = new ExternalExportInfo(true, true, true, true, context => Operation.Constant(target), null);

            var export = new DefaultExport(type, info, true);
            export.ImportMetadata = Chain.SelectImport(export);

            return export;
        }
        
        public IExport CreateMethodExport([NotNull] MethodInfo info, ref int currentLevel)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (!info.IsStatic || !DefaultExport.IsExport(info)) return null;

            var attr = info.GetCustomAttribute<ExportLevelAttribute>();
            if (attr != null) currentLevel = attr.Level;

            return new DefaultExport(
                info,
                new ExternalExportInfo(true, false, true, false, arg1 => Operation.InvokeReturn(info), info.Name),
                false);
        }
    }
}