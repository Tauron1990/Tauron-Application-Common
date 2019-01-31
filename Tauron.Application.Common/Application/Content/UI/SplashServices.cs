using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application
{
    public abstract class SplashMessage
    {

    }

    public sealed class ProgressUpdate : SplashMessage
    {
        public int Percent { get;  }

        public string Message { get; }

        public ProgressUpdate(int percent, string message)
        {
            Percent = percent;
            Message = message;
        }
    }

    public sealed class ComponentUpdate : SplashMessage
    {
        public string Component { get; }

        public ComponentUpdate(string component) => Component = component;
    }

    public sealed class SplashMessageListener : ObservableObject
    {
        public SplashMessageListener() => CurrentListner = this;

        private string _message;
        private object _splashContent;
        private double _height = 236;
        private object _mainLabelForeground = "Black";
        private object _mainLabelBackground = "White";
        private double _width = 414;
        private List<ISplashTask> _splashTasks = new List<ISplashTask>();
        private double _progress;
        private double _overallProgress;
        private UIObservableCollection<string> _components;
        private int _currentWeight;
        private double _taskweight;
        private int _progressWeight;
        private int _maximumWeight;

        [NotNull]
        public static SplashMessageListener CurrentListner { get; set; } = new SplashMessageListener();

        public double Height
        {
            get => _height;

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public object MainLabelForeground
        {
            get => _mainLabelForeground;

            set
            {
                _mainLabelForeground = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public object MainLabelBackground
        {
            get => _mainLabelBackground;

            set
            {
                _mainLabelBackground = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string Message
        {
            get => _message;

            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public object SplashContent
        {
            get => _splashContent;

            set
            {
                _splashContent = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => _width;

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public double OverallProgress
        {
            get => _overallProgress;
            set => SetProperty(ref _overallProgress, value);
        }

        public UIObservableCollection<string> Components
        {
            get => _components;
            private set => SetProperty(ref _components, value);
        }

        public void AddTask(ISplashTask task) => _splashTasks.Add(task);

        public void AddTask(int weight, Action<Action<SplashMessage>> progress) => _splashTasks.Add(new SplashTask(weight, progress));

        public void Run()
        {
            try
            {
                _maximumWeight = _splashTasks.Sum(p => p.Weight);
                Components = new UIObservableCollection<string>();

                foreach (var splashTask in _splashTasks)
                {
                    try
                    {
                        _currentWeight = splashTask.Weight;
                        splashTask.MessageRecived += MessageRecive;
                        splashTask.Run();
                        _progressWeight += splashTask.Weight;
                        _taskweight = -1;
                        CalculatePercent();
                    }
                    finally
                    {
                        splashTask.MessageRecived -= MessageRecive;
                        Components.Clear();
                    }
                }

                Components = null;
            }
            finally
            {
                _splashTasks.Clear();
            }
        }

        private void MessageRecive(SplashMessage progress)
        {
            switch (progress)
            {
                case ComponentUpdate cu:
                    Components.Add(cu.Component);
                    break;
                case ProgressUpdate pu:
                    Message = pu.Message;
                    _taskweight = _currentWeight * ((double)pu.Percent / 100);
                    CalculatePercent();
                    break;
            }
        }

        private void CalculatePercent()
        {
            double overallwight = _taskweight;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (overallwight == -1)
                overallwight = _currentWeight;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_taskweight == -1)
                Progress = 100;
            else
                Progress = 100d / _currentWeight * _taskweight;

            OverallProgress = 100d / _maximumWeight * (_progressWeight + overallwight);
        }
    }

    public interface IComponentMessager
    {
        IDisposable Subscribe(Action<ComponentUpdate> updateAction);
    }

    [PublicAPI]
    public class SplashTask : ISplashTask
    {
        public event Action<SplashMessage> MessageRecived;

        private readonly Action<Action<SplashMessage>> _progress;

        public SplashTask(int weight, Action<Action<SplashMessage>> progress)
        {
            Weight = weight;
            _progress = progress;
        }

        public void Run() => _progress(OnMessageRecived);

        public int Weight { get; }

        protected virtual void OnMessageRecived(SplashMessage obj) => MessageRecived?.Invoke(obj);
    }

    public interface ISplashTask
    {
        int Weight { get; }

        event Action<SplashMessage> MessageRecived;
        
        void Run();
    }

    [PublicAPI]
    public interface ISplashService
    {
        [NotNull]
        SplashMessageListener Listner { get; }

        void CloseSplash();

        void ShowSplash();
    }
}