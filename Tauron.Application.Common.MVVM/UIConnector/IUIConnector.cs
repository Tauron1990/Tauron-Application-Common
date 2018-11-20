using JetBrains.Annotations;
using Tauron.Application.Views.Core;

namespace Tauron.Application.UIConnector
{
    [PublicAPI]
    public interface IUIConnector
    {
        IApplicationConnector ApplicationConnector { get; }

        ICommandConnector CommandConnector { get; }

        IViewLocator ViewLocator { get; set; }

        IViewManagerConnector ViewManagerConnector { get; }
    }
}