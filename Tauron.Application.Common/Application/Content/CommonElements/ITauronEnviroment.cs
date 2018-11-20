using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface ITauronEnviroment
    {
        [NotNull]
        string DefaultProfilePath { get; set; }

        [NotNull]
        string LocalApplicationData { get; }

        [NotNull]
        string LocalApplicationTempFolder { get; }

        [NotNull]
        string LocalDownloadFolder { get; }
        
        [NotNull]
        IEnumerable<string> GetProfiles([NotNull] string application);

        [NotNull]
        string SearchForFolder(Guid id);
    }
}