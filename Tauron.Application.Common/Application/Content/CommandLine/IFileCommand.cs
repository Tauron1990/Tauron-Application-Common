using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IFileCommand
    {
        void ProcessFile([NotNull] string file);

        [NotNull]
        IShellFactory ProvideFactory();

    }
}