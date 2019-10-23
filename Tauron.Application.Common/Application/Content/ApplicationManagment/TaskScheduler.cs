using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    public interface ITaskScheduler : IDisposable
    {
        Task QueueTask([NotNull] ITask task);
    }
    
    [PublicAPI, DebuggerStepThrough]
    public sealed class TaskScheduler : ITaskScheduler
    {
        private readonly string _name;

        public bool Disposed => _disposed;
        
        private readonly BlockingCollection<ITask> _collection;
        
        private readonly IUISynchronize _synchronizationContext;
        
        private bool _disposed;

        private bool _predisposed;

        private Task _task;
        

        public TaskScheduler([NotNull] IUISynchronize synchronizationContext, string name)
        {
            _name = name;
            _synchronizationContext = Argument.NotNull(synchronizationContext, nameof(synchronizationContext));
            _collection = new BlockingCollection<ITask>();
        }


        public TaskScheduler() { }


        ~TaskScheduler() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        [NotNull]
        public Task QueueTask(ITask task)
        {
            Argument.NotNull(task, nameof(task));

            CheckDispose();
            if (task.Synchronize && _synchronizationContext != null)
                return _synchronizationContext.BeginInvoke(task.Execute);

            if (_collection == null)
            {
                CommonConstants.LogCommon(false, "Task Scheduler: Scheduler Not Initialized");
                task.Execute();
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }

            if (_collection.IsAddingCompleted) return Task.CompletedTask;
            _collection.Add(task);
            return task.Task;
        }

        internal void EnterLoop()
        {
            var source = new TaskCompletionSource<object>();
            _task = source.Task;
            EnterLoopPrivate();
            source.SetResult(null);
        }

        private void EnterLoopPrivate()
        {
            var thread = Thread.CurrentThread;
            if (string.IsNullOrWhiteSpace(thread.Name) && !string.IsNullOrWhiteSpace(_name))
                thread.Name = _name;

            foreach (var task in _collection.GetConsumingEnumerable())
                try
                {
                    task.Execute();
                }
                catch (Exception e)
                {
                    LogManager.GetLogger("TauronTaskScheduler").Error(e);
                    throw;
                }

            _collection.Dispose();
            _disposed = true;
        }

        public void Start() => _task = Task.Factory.StartNew(EnterLoopPrivate, TaskCreationOptions.LongRunning);

        private void CheckDispose()
        {
            if (_disposed)
                throw new ObjectDisposedException("TaskScheduler");
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_collection")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        // ReSharper disable UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            // ReSharper restore UnusedParameter.Local
            if (_predisposed) return;

            _predisposed = true;

            _collection?.CompleteAdding();
            _task?.Wait();
        }
    }
}