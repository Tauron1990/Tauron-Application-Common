namespace Tauron.Application.UIConnector
{
    public interface IViewManagerConnector
    {
        int GetSortOrder(object view);

        IWindow GetWindow(string name);


    }
}