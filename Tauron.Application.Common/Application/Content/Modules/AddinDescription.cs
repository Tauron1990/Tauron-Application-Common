using System;
using JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [PublicAPI]
    public sealed class AddinDescription
    {
        public AddinDescription([NotNull] Version version, [NotNull] string description, [NotNull] string name)
        {
            Version = Argument.NotNull(version, nameof(version));
            Description = Argument.NotNull(description, nameof(description));
            Name = Argument.NotNull(name, nameof(name));
        }

        [NotNull]
        public Version Version { get; }

        [NotNull]
        public string Description { get; }

        [NotNull]
        public string Name { get; }
    }
}