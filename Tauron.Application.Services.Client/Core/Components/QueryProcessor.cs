using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;
using Tauron.Application.Services.Client.Querys;

namespace Tauron.Application.Services.Client.Core.Components
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IDispatcherClient _client;

        public QueryProcessor(IDispatcherClient client) 
            => _client = client;

        public Task<TRespond> Query<TQuery, TRespond>(TQuery query) where TQuery : IQueryHelper<TRespond> where TRespond : IMessage 
            => _client.Query<TQuery, TRespond>(query);
    }
}