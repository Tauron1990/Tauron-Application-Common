using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Querys
{
    public interface IQueryProcessor
    {
        Task<TRespond> Query<TQuery, TRespond>(TQuery query)
            where TQuery : IQueryHelper<TRespond>
            where TRespond : IQueryResult;
    }
}