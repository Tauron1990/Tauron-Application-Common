using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc
{
    public sealed class DefaultContainer : ComponentMessagnerBase, IContainer
    {
        public DefaultContainer()
        {
            _extensions = new List<IContainerExtension>();
            _componetnts = new ComponentRegistry();
            _exports = new ExportRegistry();
            _exportproviders = new ExportProviderRegistry();
            _exportproviders.ExportsChanged += ExportsChanged;
            Register(new DefaultExtension());
            _buildEngine = new BuildEngine(this, _exportproviders, _componetnts);
            _exports.Register(DefaultExportFactory.Factory.CreateAnonymosWithTarget(typeof(IContainer), this), 0);
        }

        private void ExportsChanged(object sender, ExportChangedEventArgs e)
        {
            var temp = new List<IExport>();

            foreach (var exportMetadata in
                e.Removed.Where(exportMetadata => !temp.Contains(exportMetadata.Export)))
            {
                _exports.Remove(exportMetadata.Export);
                temp.Add(exportMetadata.Export);
            }

            temp.Clear();

            foreach (
                var exportMetadata in e.Added.Where(exportMetadata => !temp.Contains(exportMetadata.Export)))
            {
                var attr = exportMetadata.Export.ImplementType.GetCustomAttribute<ExportLevelAttribute>() ?? exportMetadata.Export.ImplementType.Assembly.GetCustomAttribute<ExportLevelAttribute>();

                _exports.Register(exportMetadata.Export, attr?.Level ?? 0);
                temp.Add(exportMetadata.Export);
            }
        }

        private readonly BuildEngine _buildEngine;

        private readonly ComponentRegistry _componetnts;

        private readonly ExportProviderRegistry _exportproviders;

        private readonly ExportRegistry _exports;

        private readonly List<IContainerExtension> _extensions;

        public IRightable DeferBuildUp(ExportMetadata data, ErrorTracer errorTracer, SubCompilitionUnit subUnit, params BuildParameter[] parameters)
        {
            try
            {
                subUnit.VariableNamer.AddLevel();
                var op = _buildEngine.CreateOperationBlock(data, errorTracer, subUnit, parameters);
                subUnit.AddCode(op);

                return Operation.Variable(subUnit.TargetName);
            }
            finally
            {
                subUnit.VariableNamer.RemoveLevel();
            }
        }
        //=> () =>
            //{
            //    errorTracer.Phase = string.Empty;
            //    errorTracer.Phase = data.ToString();

            //    var result = _buildEngine.CreateDelegate(data.Export, data.ContractName, errorTracer, parameters, false, out _)(null);
            //    if (errorTracer.Exceptional)
            //        throw new BuildUpException(errorTracer);
            //    return result;
            //};

        [NotNull]
        public object BuildUp([NotNull] ExportMetadata data, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            Argument.NotNull(data, nameof(data));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                errorTracer.Export = data.ToString();
                return _buildEngine.BuildUp(data.Export, data.ContractName, errorTracer, parameters);
            }
            catch (Exception e)
            {
                if (e is BuildUpException) throw;

                errorTracer.Exception = e;

                throw new BuildUpException(errorTracer);
            }
        }

        public object BuildUp(object toBuild, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            Argument.NotNull(toBuild, nameof(toBuild));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                return _buildEngine.BuildUp(toBuild, errorTracer, parameters, _exports.TryFindExport(toBuild));
            }
            catch (Exception e)
            {
                if (e is BuildUpException) throw;

                errorTracer.Exception = e;

                throw new BuildUpException(errorTracer);
            }
        }

        public object BuildUp(Type type, ErrorTracer errorTracer, BuildParameter[] buildParameters, params object[] constructorArguments)
        {
            Argument.NotNull(type, nameof(type));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                return _buildEngine.BuildUp(type, constructorArguments, errorTracer, buildParameters);
            }
            catch (Exception e)
            {
                if (e is BuildUpException) throw;

                errorTracer.Exception = e;
                throw new BuildUpException(errorTracer);
            }
        }
        
        public void Dispose()
        {
            _componetnts.Dispose();
            _exportproviders.Dispose();
        }

        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional)
        {
            Argument.NotNull(interfaceType, nameof(interfaceType));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                return isOptional ? _exports.FindOptional(interfaceType, name, errorTracer) : FindExport(interfaceType, name, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                //if (e is FindExportException) throw;

                //throw new FindExportException(string.Format("Resolve Failed: [{0}|{1}]", interfaceType, name), e);
            }

            return null;
        }

        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer)
        {
            Argument.NotNull(interfaceType, nameof(interfaceType));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                return _exports.FindSingle(interfaceType, name, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;

                //if (e is FindExportException) throw;

                //throw new FindExportException(string.Format("Resolve Failed: [{0}|{1}]", interfaceType, name), e);
            }

            return null;
        }

        public IEnumerable<ExportMetadata> FindExports(Type interfaceType, string name, ErrorTracer errorTracer)
        {
            Argument.NotNull(interfaceType, nameof(interfaceType));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            try
            {
                return _exports.FindAll(interfaceType, name, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;

                //if (e is FindExportException) throw;

                //throw new FindExportException(string.Format("Resolve Failed: [{0}|{1}]", interfaceType, name), e);
            }

            return Enumerable.Empty<ExportMetadata>();
        }

        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional, int level) => Argument.CheckResult(isOptional
            ? _exports.FindOptional(interfaceType, name, errorTracer, level)
            : _exports.FindSingle(interfaceType, name, errorTracer, level), "Result Was null");

        public IEnumerable<ExportMetadata> FindExports(Type interfaceType, string name, ErrorTracer errorTracer, int level) => _exports.FindAll(interfaceType, name, errorTracer, level);

        public void Register(IExport exportType, int level)
        {
            _exports.Register(exportType, level);
            foreach (var metadata in exportType.ExportMetadata)
                Publish(metadata.ToString());
        }

        public void Register(ExportResolver exportResolver) => exportResolver.Fill(_componetnts, _exports, _exportproviders, AddComponent);

        private void AddComponent(ExportMetadata meta) => Publish(meta.ToString());

        public void Register(IContainerExtension extension)
        {
            Argument.NotNull(extension, nameof(extension));

            extension.Initialize(_componetnts);
            _extensions.Add(extension);
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            Argument.NotNull(serviceType, nameof(serviceType));
            ErrorTracer tracer = new ErrorTracer();

            var exp = FindExport(serviceType, null, tracer, true);
            if (exp == null) return null;

            return BuildUp(exp, tracer);
        }
    }
}