using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Commands;
using Tauron.Application.Models;

namespace Tauron.Application
{
    public abstract class TabWorkspace : ViewModelBase, ITabWorkspace
    {
        protected const string CloseEventName = "CloseEvent";

        protected TabWorkspace([NotNull] string title, string name = null)
        {
            _title = title;
            _name = name;
            _canClose = true;
            CloseWorkspace = new SimpleCommand(obj => CanClose, obj => InvokeClose());
        }

        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ITabWorkspace> Close;

        public virtual void InvokeClose() => Close?.Invoke(this);

        private bool _canClose;

        private string _title;
        private readonly string _name;

        public bool CanClose
        {
            get => _canClose;

            set
            {
                _canClose = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseWorkspace { get; private set; }

        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Name => _name ?? _title;

        public virtual void OnClose() { }

        public virtual void OnActivate() { }

        public virtual void OnDeactivate() { }
        
    }
}