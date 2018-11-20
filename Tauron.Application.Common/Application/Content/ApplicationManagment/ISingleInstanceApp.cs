using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ISingleInstanceApp
    {
        bool SignalExternalCommandLineArgs([NotNull] IList<string> args);
    }
}