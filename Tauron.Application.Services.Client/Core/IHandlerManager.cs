using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.Services.Client.Core
{
    public interface IHandlerManager
    {
        Task Init();
    }
}