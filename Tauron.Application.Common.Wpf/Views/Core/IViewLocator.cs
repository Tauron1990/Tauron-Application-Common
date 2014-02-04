using System;
using System.Windows;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views.Core
{
    [PublicAPI]
    public interface IViewLocator
    {
        [CanBeNull]
        DependencyObject CreateViewForModel([NotNull] object model);

        [CanBeNull]
        DependencyObject CreateViewForModel([NotNull] Type model);

        [CanBeNull]
        DependencyObject CreateView([NotNull] string name);

        [CanBeNull]
        Window CreateWindow([NotNull] string name);

    }
}