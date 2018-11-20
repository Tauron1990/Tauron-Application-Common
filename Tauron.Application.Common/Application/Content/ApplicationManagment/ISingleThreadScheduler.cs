using System;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ISingleThreadScheduler
    {
        bool IsBackground { get; set; }

        void Queue([NotNull] Action task);
    }
}