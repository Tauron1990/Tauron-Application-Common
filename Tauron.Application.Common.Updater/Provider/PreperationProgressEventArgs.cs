using System;

namespace Tauron.Application.Common.Updater.Provider
{
    public sealed class PreperationProgressEventArgs : EventArgs
    {
        public double Percent { get; }

        public PreperationProgressEventArgs(double percent) => Percent = percent;
    }
}