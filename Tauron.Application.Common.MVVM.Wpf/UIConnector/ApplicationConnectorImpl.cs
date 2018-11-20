using System.ComponentModel;
using System.Windows;
using Tauron.Application.Models.Interfaces;

namespace Tauron.Application.UIConnector
{
    public sealed class ApplicationConnectorImpl : IApplicationConnector
    {
        private bool? _isInDesignMode;

        public ApplicationConnectorImpl(System.Windows.Application application)
        {
            Dispatcher = new DispatcherInterfaceImpl(application.Dispatcher);
            Application = new ApplicationInstanceImpl(application);
        }

        public bool IsInDesignMode
        {
            get
            {
                if (_isInDesignMode.HasValue) return _isInDesignMode.Value;
                var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement));
                _isInDesignMode = (bool)dependencyPropertyDescriptor.Metadata.DefaultValue;
                return _isInDesignMode.Value;
            }
        }

        public IDispatcher Dispatcher { get; }

        public IApplication Application { get; }
    }
}
