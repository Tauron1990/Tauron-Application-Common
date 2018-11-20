using JetBrains.Annotations;

namespace Tauron.Application.Common.Updater.Provider
{
    [PublicAPI]
    public sealed class ReleaseFile
    {
        public ReleaseFile(string name, string location)
        {
            Name = Argument.NotNull(name, nameof(name));
            Location = Argument.NotNull(location, nameof(location));
        }

        public string Location { get; }

        public string Name { get; }
    }
}