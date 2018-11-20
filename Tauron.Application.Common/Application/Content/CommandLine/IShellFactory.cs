using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IShellFactory
    {
        [NotNull]
        object CreateView();
    }
}