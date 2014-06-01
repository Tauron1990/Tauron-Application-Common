using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    public sealed class AddinDescription
    {
        [NotNull]
        public Version Version { get; set; }

        [NotNull]
        public string Description { get; set; }

        [NotNull]
        public string Name { get; set; }

        public AddinDescription([NotNull] Version version, [NotNull] string description, [NotNull] string name)
        {
            Contract.Requires<ArgumentNullException>(version != null, "version");
            Contract.Requires<ArgumentNullException>(description != null, "description");
            Contract.Requires<ArgumentNullException>(name != null, "name");

            Version = version;
            Description = description;
            Name = name;
        }
    }
}
