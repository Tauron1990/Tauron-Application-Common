using System.Windows;
using Tauron.Application.Views;

namespace Tauron.Application.UIConnector
{
    public class ViewManagerConnector : IViewManagerConnector
    {
        public int GetSortOrder(object view)
        {
            if(view is DependencyObject dependencyObject)
                return ViewManagerHelper.GetSortOrder(dependencyObject);
            return 0;
        }

        public IWindow GetWindow(string name)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if(window.Name == name)
                    return new WpfWindow(window);
            }

            return null;
        }
    }
}