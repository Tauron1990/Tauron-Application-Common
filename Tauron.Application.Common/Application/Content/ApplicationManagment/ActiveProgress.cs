using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public class ActiveProgress
    {
        public ActiveProgress([NotNull] string message, double percent, double overAllProgress)
        {
            if (percent < 0) percent = 0;
            if (overAllProgress < 0) overAllProgress = 0;

            if (percent > 100 || double.IsNaN(percent)) percent = 100;

            if (overAllProgress > 100 || double.IsNaN(overAllProgress)) overAllProgress = 100;

            Message = message;
            Percent = percent;
            OverAllProgress = overAllProgress;
        }

        [NotNull]
        public string Message { get; private set; }

        public double OverAllProgress { get; set; }

        public double Percent { get; private set; }
    }
}