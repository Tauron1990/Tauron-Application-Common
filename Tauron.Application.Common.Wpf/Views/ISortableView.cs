namespace Tauron.Application.Views
{
    public interface ISortableViewExportMetadata : INameExportMetadata
    {
         int Order { get; }
    }
}