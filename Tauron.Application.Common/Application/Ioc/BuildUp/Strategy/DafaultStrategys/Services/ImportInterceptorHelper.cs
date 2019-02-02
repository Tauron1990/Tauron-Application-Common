using System.Reflection;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ImportInterceptorHelper
    {
        private readonly IImportInterceptor _interceptor;
        private readonly MemberInfo _member;
        private readonly ImportMetadata _metadata;

        public ImportInterceptorHelper([NotNull] IImportInterceptor interceptor, [NotNull] MemberInfo member,
            [NotNull] ImportMetadata metadata)
        {
            _interceptor = Argument.NotNull(interceptor, nameof(interceptor));
            _member = Argument.NotNull(member, nameof(member));
            _metadata = Argument.NotNull(metadata, nameof(metadata));
        }

        public (Condition IsOK, ICodeLine Operation) Intercept(string variable) => _interceptor.Intercept(_member, _metadata, variable);
    }
}