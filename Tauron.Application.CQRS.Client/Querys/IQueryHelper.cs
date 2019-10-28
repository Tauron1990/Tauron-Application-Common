namespace Tauron.Application.CQRS.Client.Querys
{
    public interface IQueryHelper<TRespond> : IQuery
        where TRespond : IQueryResult
    {
        
    }
}