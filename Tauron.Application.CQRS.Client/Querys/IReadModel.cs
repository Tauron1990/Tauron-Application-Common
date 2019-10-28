using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Querys
{
    public interface IReadModel<in TQuery, TRespond>
        where TQuery : IQueryHelper<TRespond>
        where TRespond : IQueryResult
    {
        Task<TRespond> Query(TQuery query);
    }
}