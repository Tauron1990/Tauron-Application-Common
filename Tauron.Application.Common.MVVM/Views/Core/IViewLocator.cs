using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    [PublicAPI]
    public interface IViewLocator
    {
        void Register([NotNull] ExportNameHelper export);

        [CanBeNull]
        object CreateViewForModel([NotNull] object model);

        [CanBeNull]
        object CreateViewForModel([NotNull] Type model);

        [NotNull]
        object CreateView([NotNull] string name);

        [NotNull]
        IWindow CreateWindow([NotNull] string name, [CanBeNull] object[] parameters);

        [NotNull]
        Type GetViewType([NotNull] string name);

        [NotNull]
        IEnumerable<object> GetAllViews([NotNull] string name);

        [CanBeNull]
        string GetName([NotNull] ViewModelBase model);
    }
}