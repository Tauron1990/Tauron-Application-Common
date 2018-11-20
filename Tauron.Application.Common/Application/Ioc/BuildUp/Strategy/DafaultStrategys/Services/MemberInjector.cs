using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public abstract class MemberInjector
    {
        public abstract void Inject([NotNull] object target, [NotNull] IContainer container, [NotNull] ImportMetadata metadata, [CanBeNull] IImportInterceptor interceptor,
            [NotNull] ErrorTracer errorTracer,
            [CanBeNull] BuildParameter[] parameters);
    }
}