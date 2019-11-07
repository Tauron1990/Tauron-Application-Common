using System;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Querys;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public sealed class QueryAwaiter<TRespond> : IEventHandler<TRespond>, IDisposable 
        where TRespond : IQueryResult
    {
        private readonly IDisposable _disposable;
        private readonly ManualResetEvent _asyncManualReset;

        private TRespond _respond;

        public QueryAwaiter(GlobalEventHandler<TRespond> globalEventHandler)
        {
            _disposable = globalEventHandler.Register(this, t => Handle(t));
            _asyncManualReset = new ManualResetEvent(false);
        }


        public Task Handle(TRespond message)
        {
            _respond = message;

            _asyncManualReset.Set();

            return Task.CompletedTask;
        }

        public async Task<TRespond> SendQuery(IQueryHelper<TRespond> query, Func<IQueryHelper<TRespond>, Task> sender)
        {
            _asyncManualReset.Reset();

            await sender(query);
            _asyncManualReset.WaitOne(30_000);

            return _respond;
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _asyncManualReset.Dispose();
        }
    }
}