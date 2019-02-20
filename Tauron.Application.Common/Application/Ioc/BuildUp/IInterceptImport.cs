using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI]
    public interface IImportInterceptor
    {
        (Expression IsOK, Expression Operation) Intercept([NotNull] MemberInfo member, [NotNull] ImportMetadata metadata, [NotNull] string targetVariable);
    }
}