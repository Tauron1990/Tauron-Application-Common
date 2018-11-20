using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application.Composition
{
    public interface IViewAggregatorAdapter
    {
        void Adapt([NotNull] DependencyObject dependencyObject);

        void AddViews([NotNull] IEnumerable<object> views);

        bool CanAdapt([NotNull] DependencyObject dependencyObject);

        void Release();
    }
}