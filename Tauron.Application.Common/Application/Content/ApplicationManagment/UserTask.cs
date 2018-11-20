using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [DebuggerStepThrough]
    public class UserResultTask<TResult> : ITask
    {
        public UserResultTask([NotNull] Func<TResult> callback, bool sync)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Synchronize = sync;
            _task = new TaskCompletionSource<TResult>();
        }
        
        public void Execute()
        {
            try
            {
                _task.SetResult(_callback());
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }
        
        private readonly Func<TResult> _callback;

        private readonly TaskCompletionSource<TResult> _task;
        
        public bool Synchronize { get; }
        
        public Task Task => _task.Task;
        
    }
    
    [DebuggerStepThrough]
    public class UserTask : ITask
    {
        public UserTask([NotNull] Action callback, bool sync)
        {
            _callback = Argument.NotNull(callback, nameof(callback));
            Synchronize = sync;
            _task = new TaskCompletionSource<object>();
        }
        
        public void Execute()
        {
            try
            {
                _callback();
                _task.SetResult(null);
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }
        
        private readonly Action _callback;

        private readonly TaskCompletionSource<object> _task;
        
        public bool Synchronize { get; }
        
        public Task Task => _task.Task;
    }
}