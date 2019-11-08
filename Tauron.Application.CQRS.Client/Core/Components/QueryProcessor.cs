using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Querys;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IDispatcherClient _client;

        public QueryProcessor(IDispatcherClient client) 
            => _client = client;

        public Task<TRespond?> Query<TQuery, TRespond>(TQuery query) where TQuery : IQueryHelper<TRespond> where TRespond : class, IQueryResult 
            => _client.Query<TQuery, TRespond>(query);
    }
}