using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class InjectMemberPolicy : IPolicy
    {
        [NotNull]
        public ImportMetadata Metadata { get;  }

        [NotNull]
        public MemberInjector Injector { get;  }

        [CanBeNull]
        public List<IImportInterceptor> Interceptors { get; set; }

        public InjectMemberPolicy([NotNull] ImportMetadata metadata, [NotNull] MemberInjector injector)
        {
            Metadata = Argument.NotNull(metadata, nameof(metadata));
            Injector = Argument.NotNull(injector, nameof(injector));
        }
    }
}