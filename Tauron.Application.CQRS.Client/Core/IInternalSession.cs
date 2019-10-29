using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Domain;

namespace Tauron.Application.CQRS.Client.Core
{
    public interface IInternalSession : ISession
    {
        Task Commit();
    }
}