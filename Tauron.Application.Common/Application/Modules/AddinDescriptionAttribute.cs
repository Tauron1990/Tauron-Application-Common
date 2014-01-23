using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [AttributeUsage(AttributeTargets.Class), PublicAPI]
    public sealed class AddinDescriptionAttribute : ExportModuleAttribute
    {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public Version Version { get; set; }

        [CanBeNull]
        public string Description { get; set; }

        public AddinDescriptionAttribute([NotNull] string name, [NotNull] string version)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (version == null) throw new ArgumentNullException("version");
            Name = name;

            Version ver;
            Version = Version.TryParse(version, out ver) ? ver : new Version();
        }
    }
}
