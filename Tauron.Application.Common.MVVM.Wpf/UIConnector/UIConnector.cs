using Tauron.Application.Ioc;
using Tauron.Application.Views.Core;

namespace Tauron.Application.UIConnector
{
    [Export(typeof(IUIConnector))]
    public class UIConnector : IUIConnector
    {
        public IApplicationConnector ApplicationConnector { get; } = new ApplicationConnectorImpl(System.Windows.Application.Current);
        public ICommandConnector CommandConnector { get; } = new CommandConnector();
        public IViewLocator ViewLocator { get; set; } = Factory.Object<AttributeBasedLocator>();
        public IViewManagerConnector ViewManagerConnector { get; } = new ViewManagerConnector();
    }
}