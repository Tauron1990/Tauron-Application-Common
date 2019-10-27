using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Commands
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task<OperationResult> Handle(TCommand command);
    }
}