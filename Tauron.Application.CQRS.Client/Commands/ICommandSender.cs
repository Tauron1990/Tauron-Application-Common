using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Commands
{
    public interface ICommandSender
    {
        Task<OperationResult> Send<TType>(TType command)
            where TType : ICommand;
    }
}