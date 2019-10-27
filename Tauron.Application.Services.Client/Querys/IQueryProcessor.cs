using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Querys
{
    public interface IQueryProcessor
    {
        Task<TRespond> Query<TQuery, TRespond>(TQuery query)
            where TQuery : IQueryHelper<TRespond>
            where TRespond : IMessage;
    }
}