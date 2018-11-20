using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ITabWorkspace
    {
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action<ITabWorkspace> Close;

        bool CanClose { get; set; }

        [NotNull]
        ICommand CloseWorkspace { get; }

        [NotNull]
        string Title { get; set; }

        void OnClose();

        void OnActivate();

        void OnDeactivate();
    }
}