using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI, ContractClass(typeof(ImportInterceptorFactoryContracts))]
    public interface IImportInterceptorFactory
    {
        [CanBeNull]
        IImportInterceptor CreateInterceptor([NotNull] ExportMetadata export);
    }

    [ContractClassFor(typeof(IImportInterceptorFactory))]
    abstract class ImportInterceptorFactoryContracts : IImportInterceptorFactory
    {
        public IImportInterceptor CreateInterceptor(ExportMetadata export)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");
            return null;
        }
    }
}