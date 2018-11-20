using System;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public enum ProgressStyle
    {
        None,
        ProgressBar,
        MarqueeProgressBar
    }

    [PublicAPI]
    public interface IProgressDialog : IDisposable
    {
        ProgressStyle ProgressBarStyle { get; set; }
        event EventHandler Completed;
        void Start();
    }
}