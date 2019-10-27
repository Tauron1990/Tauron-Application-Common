using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Commands;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Core.Components
{
    public class CommandSender : ICommandSender
    {
        private readonly IDispatcherClient _dispatcher;

        public CommandSender(IDispatcherClient api) => _dispatcher = api;

        public Task<OperationResult> Send<TType>(TType command) where TType : ICommand 
            => _dispatcher.SendCommand(command);
    }
}