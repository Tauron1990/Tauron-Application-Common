using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Commands
{
    public interface ICommandSender
    {
        Task<OperationResult> Send<TType>(TType command)
            where TType : ICommand;
    }
}