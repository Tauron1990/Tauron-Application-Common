using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class CommandSender : ICommandSender
    {
        private readonly IDispatcherClient _dispatcher;

        public CommandSender(IDispatcherClient api) => _dispatcher = api;

        public Task<OperationResult> Send<TType>(TType command) where TType : ICommand 
            => _dispatcher.SendCommand(command);
    }
}