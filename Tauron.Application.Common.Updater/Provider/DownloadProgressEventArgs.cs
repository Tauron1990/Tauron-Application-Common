using System;

namespace Tauron.Application.Common.Updater.Provider
{
    public sealed class DownloadProgressEventArgs : EventArgs
    {
        public double Percent { get; }

        public DownloadProgressEventArgs(double percent) => Percent = percent;
    }
}