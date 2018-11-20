using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ICommandLineService
    {
        [NotNull]
        IEnumerable<ICommandLineCommand> Commands { get; }

        void Add([NotNull] ICommandLineCommand command);
    }
}