using System;
using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Core
{
    public class ErrorManager : IErrorManager
    {
        public event Func<string, Task>? ConnectionFailedEvent; 

        public Task ConnectionFailed(string message) 
            => ConnectionFailedEvent?.Invoke(message) ?? Task.CompletedTask;
    }
}