using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI, ContractClass(typeof(ImportInterceptorContracts))]
    public interface IImportInterceptor
    {
        bool Intercept([NotNull] MemberInfo member, [NotNull] ImportMetadata metadata, [NotNull] object target, [CanBeNull] ref object value);
    }

    [ContractClassFor(typeof(IImportInterceptor))]
    abstract class ImportInterceptorContracts : IImportInterceptor
    {
        public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(target != null, "target");

            return false;
        }
    }
}