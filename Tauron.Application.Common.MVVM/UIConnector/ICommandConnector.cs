using System;

namespace Tauron.Application.UIConnector
{
    public interface ICommandConnector
    {
       Action<EventHandler> Add { get; }
       Action<EventHandler> Remove { get; }
       Action InvalidateRequerySuggested { get; }
    }
}