using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class ImportInterceptorHelper
    {
        private readonly IImportInterceptor _interceptor;
        private readonly MemberInfo _member;
        private readonly ImportMetadata _metadata;
        private readonly object _target;

        public ImportInterceptorHelper([NotNull] IImportInterceptor interceptor, [NotNull] MemberInfo member,
            [NotNull] ImportMetadata metadata, [NotNull] object target)
        {
            _interceptor = Argument.NotNull(interceptor, nameof(interceptor));
            _member = Argument.NotNull(member, nameof(member));
            _metadata = Argument.NotNull(metadata, nameof(metadata));
            _target = Argument.NotNull(target, nameof(target));
        }

        public bool Intercept([CanBeNull] ref object value) => _interceptor.Intercept(_member, _metadata, _target, ref value);
    }
}