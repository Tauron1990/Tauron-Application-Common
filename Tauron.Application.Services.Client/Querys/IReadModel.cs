using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Querys
{
    public interface IReadModel<TQuery, TRespond>
        where TQuery : IQueryHelper<TRespond>
        where TRespond : IQueryResult
    {
        Task<TRespond> Query(TQuery query);
    }
}