using System;
using System.Windows.Input;

namespace Tauron.Application.UIConnector
{
    public class CommandConnector : ICommandConnector
    {
        public CommandConnector()
        {
            Add = handler => CommandManager.RequerySuggested += handler;
            Remove = handler => CommandManager.RequerySuggested -= handler;
            InvalidateRequerySuggested = CommandManager.InvalidateRequerySuggested;
        }

        public Action<EventHandler> Add { get; }
        public Action<EventHandler> Remove { get; }
        public Action InvalidateRequerySuggested { get; }
    }
}