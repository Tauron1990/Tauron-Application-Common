using System;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Querys;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public sealed class SimpleAwaiter<TRespond> : IEventHandler<TRespond>, IDisposable
        where TRespond : class, IEvent
    {
        private readonly IDisposable _disposable;
        private readonly ManualResetEvent _asyncManualReset;

        private TRespond? _respond;

        public SimpleAwaiter(GlobalEventHandler<TRespond> globalEventHandler)
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

        public Task<TRespond?> Wait()
        {
            _asyncManualReset.Reset();

            _asyncManualReset.WaitOne(30_000);

            return Task.FromResult(_respond);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _asyncManualReset.Dispose();
        }
    }
}