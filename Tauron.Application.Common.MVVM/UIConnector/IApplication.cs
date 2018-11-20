using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Interfaces
{
    [PublicAPI]
    public interface IApplication
    {
        object FindResource(object resourceKey);

        object TryFindResource(object resourceKey);

        IEnumerable<IWindow> Windows { get; }

        IWindow MainWindow { get; set; }

        event EventHandler Exit;
    }
}