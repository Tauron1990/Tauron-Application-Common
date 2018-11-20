using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The strategy base.</summary>
    public abstract class StrategyBase : IStrategy
    {
        public virtual void Initialize([NotNull] ComponentRegistry components)
        {
        }

        public virtual void OnBuild(IBuildContext context)
        {
        }

        public virtual void OnCreateInstance(IBuildContext context)
        {
        }

        public virtual void OnPerpare(IBuildContext context)
        {
        }

        public virtual void OnPostBuild(IBuildContext context)
        {
        }
    }
}