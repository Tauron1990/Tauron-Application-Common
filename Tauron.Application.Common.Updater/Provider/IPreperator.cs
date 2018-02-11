using System;
using System.Collections.Generic;

namespace Tauron.Application.Common.Updater.Provider
{
    public interface IPreperator
    {
        event EventHandler<PreperationProgressEventArgs> PreperationInProgressEvent; 

        string Prepare(string path);

        void ExtractFiles(string source, string target);
    }
}