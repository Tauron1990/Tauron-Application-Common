using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Querys
{
    public interface IQueryHelper<TRespond> : IQuery
        where TRespond : IQueryResult
    {
        
    }
}