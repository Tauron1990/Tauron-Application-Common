using System;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class InjectorContext
    {
        [NotNull]
        public ReflectionContext ReflectionContext { get; private set; }

        [CanBeNull]
        public BuildParameter[] BuildParameters { get; set; }

        [NotNull]
        public object MemberInfo { get; private set; }

        [NotNull]
        public ErrorTracer Tracer { get; set; }

        [NotNull]
        public ImportMetadata Metadata { get; set; }

        [CanBeNull]
        public IResolver Resolver { get; set; }

        [NotNull]
        public ResolverFactory Machine { get; set; }

        [CanBeNull]
        public IImportInterceptor ImportInterceptor { get; set; }

        [NotNull]
        public IContainer Container { get; set; }

        [NotNull]
        public object Target { get; set; }

        public InjectorContext([NotNull] IMetadataFactory metadataFactory, [NotNull] object memberInfo, [NotNull] Type memberType)
        {
            ReflectionContext = new ReflectionContext(metadataFactory, memberType, this);
            MemberInfo = memberInfo;
        }
    }
}