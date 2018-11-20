using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public interface IStrategy : IInitializeable
    {
        void OnBuild([NotNull] IBuildContext context);

        void OnCreateInstance([NotNull] IBuildContext context);

        void OnPerpare([NotNull] IBuildContext context);

        void OnPostBuild([NotNull] IBuildContext context);
    }
}