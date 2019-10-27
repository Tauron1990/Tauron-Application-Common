using System.Threading.Tasks;
using Tauron.Application.Services.Client.Domain;

namespace Tauron.Application.Services.Client.Core
{
    public interface IIternalSession : ISession
    {
        Task Commit();
    }
}