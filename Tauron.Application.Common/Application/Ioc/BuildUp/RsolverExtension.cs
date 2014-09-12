using System;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp
{
    public interface IResolverExtension
    {
        [NotNull]
        Type TargetType { get; }

        [CanBeNull]
        object Progress([NotNull] ExportMetadata metadata, [NotNull] object export);
    }
}