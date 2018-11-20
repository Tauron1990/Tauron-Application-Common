using System;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application
{
    internal abstract class PipelineBase : IPipeLine, ITask
    {
        protected PipelineBase([NotNull] DependencyObject target, bool simpleMode)
        {
            Source = new WeakReference<DependencyObject>(Argument.NotNull(target, nameof(target)));
            SimpleMode = simpleMode;
            _task = new TaskCompletionSource<object>();
            _task.SetResult(null);
        }
        
        public bool SimpleMode
        {
            get => _simpleMode;
            set
            {
                _simpleMode = value;
                var target = Target;

                if (SimpleMode)
                {
                    DataContextServices.UnregisterHandler(target, this);
                    var cont = new FrameworkObject(target, false).DataContext;
                    DataContext = cont == null ? null : new WeakReference(cont);
                    TaskScheduler = CommonApplication.Scheduler;
                }
                else
                {
                    DataContext = null;
                    DataContextServices.RegisterHandler(target, this);
                }
            }
        }
        
        protected virtual void DataContextChanged() { }
        
        private readonly TaskCompletionSource<object> _task;
        private bool _simpleMode;
        
        [CanBeNull]
        public WeakReference<DependencyObject> Source { get; }
        
        [CanBeNull]
        public DependencyObject Target => Source?.TypedTarget();
        
        public WeakReference DataContext { get; set; }


        public bool IsAlive => Source?.IsAlive() ?? false;


        public ITaskScheduler TaskScheduler { get; set; }

        public virtual bool Synchronize => true;


        public Task Task => _task.Task;
        
        ITask IPipeLine.Generate(WeakReference dataContext, ITaskScheduler scheduler)
        {
            DataContext = dataContext;
            TaskScheduler = scheduler;
            return this;
        }

        void ITask.Execute() => DataContextChanged();
    }
}