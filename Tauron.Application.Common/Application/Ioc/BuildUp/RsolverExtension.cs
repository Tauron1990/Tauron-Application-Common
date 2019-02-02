using System;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    public interface IResolverExtension
    {
        [NotNull]
        Type TargetType { get; }

        [CanBeNull]
        ICodeLine Progress([NotNull] ExportMetadata metadata, string variableName);
    }
}