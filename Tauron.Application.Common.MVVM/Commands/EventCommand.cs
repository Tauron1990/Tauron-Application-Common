using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Tauron.Application.Commands
{
    [PublicAPI]
    public sealed class EventCommand : CommandBase
    {
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, bool> CanExecuteEvent;

        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object> ExecuteEvent;

        public override bool CanExecute(object parameter) => OnCanExecute(parameter);
        public override void Execute(object parameter) => OnExecute(parameter);

        private bool OnCanExecute([CanBeNull] object parameter)
        {
            var handler = CanExecuteEvent;
            return handler == null || handler(parameter);
        }

        private void OnExecute([CanBeNull] object parameter) => ExecuteEvent?.Invoke(parameter);
    }
}