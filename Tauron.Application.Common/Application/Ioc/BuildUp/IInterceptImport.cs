using System.Reflection;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI]
    public interface IImportInterceptor
    {
        (Condition IsOK, ICodeLine Operation) Intercept([NotNull] MemberInfo member, [NotNull] ImportMetadata metadata, [NotNull] string targetVariable);
    }
}