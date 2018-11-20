using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [Serializable]
    [PublicAPI]
    public sealed class BuildUpException : Exception
    {
        public BuildUpException([NotNull]ErrorTracer errorTracer)
            : base(errorTracer?.Phase + errorTracer?.Export, errorTracer?.Exception)
        {
            Argument.NotNull(errorTracer, nameof(errorTracer));
            ErrorTracer = errorTracer;
        }

        [NotNull]
        public ErrorTracer ErrorTracer { get; private set; }
    }
}