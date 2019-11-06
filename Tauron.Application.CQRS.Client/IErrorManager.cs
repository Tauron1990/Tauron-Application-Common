using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client
{
    public interface IErrorManager
    {
        Task ConnectionFailed(string message);
    }
}