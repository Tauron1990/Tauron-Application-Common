using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ICommandLineCommand
    {
        void Execute([NotNull] string[] args, [NotNull] IContainer container);

        [NotNull]
        string CommandName { get; }

        [CanBeNull]
        IShellFactory Factory { get; }
    }
}