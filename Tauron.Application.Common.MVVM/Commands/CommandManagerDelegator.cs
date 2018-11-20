using System;
using Tauron.Application.UIConnector;

namespace Tauron.Application.Commands
{
    public static class CommandManagerDelegator
    {
        private static ICommandConnector _commandConnector;
        private static ICommandConnector CommandConnector 
            => _commandConnector ?? (_commandConnector = CommonApplication.Current.Container.Resolve<IUIConnector>().CommandConnector);
        
        internal static void Add(EventHandler handler) => CommandConnector.Add?.Invoke(handler);

        internal static void Remove(EventHandler handler) => CommandConnector.Remove?.Invoke(handler);

        internal static void InvalidateRequerySuggested() => CommandConnector.InvalidateRequerySuggested?.Invoke();
    }
}
