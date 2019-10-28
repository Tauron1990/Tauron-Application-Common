using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Core
{
    public interface IHandlerManager
    {
        Task Init();
    }
}