using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    public interface IResolverExtension
    {
        [NotNull]
        Type TargetType { get; }

        [CanBeNull]
        Expression Progress([NotNull] ExportMetadata metadata, string variableName);
    }
}