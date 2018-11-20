using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Implement
{
    [PublicAPI]
    [Export(typeof(ISingleThreadScheduler))]
    [NotShared]
    public sealed class SingleThreadScheduler : ISingleThreadScheduler, IDisposable
    {
        public bool IsBackground
        {
            get => _thread.IsBackground;
            set => _thread.IsBackground = value;
        }

        private void EnterLoop()
        {
            foreach (var item in _tasks.GetConsumingEnumerable())
                item();
            _tasks.Dispose();
        }

        private readonly BlockingCollection<Action> _tasks = new BlockingCollection<Action>();
        
        private readonly Thread _thread;
        
        public SingleThreadScheduler()
        {
            _thread = new Thread(EnterLoop);
            IsBackground = true;

            _thread.Start();
        }

        ~SingleThreadScheduler() => Dispose();

        public void Dispose()
        {
            _tasks.CompleteAdding();
            GC.SuppressFinalize(this);
        }
        
        public void Queue(Action task) => _tasks.Add(Argument.NotNull(task, nameof(task)));
    }
}