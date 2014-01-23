using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [PublicAPI]
    public sealed class AddIn
    {
        private string _name;
        private Version _version;

        [NotNull]
        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _name;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _name = value;
            }
        }

        [NotNull]
        public Version Version
        {
            get
            {
                Contract.Ensures(Contract.Result<Version>() != null);

                return _version;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(Version != null, "Version");

                _version = value;
            }
        }

        [CanBeNull]
        public string Description { get; set; }

        public AddIn([NotNull] string name, [NotNull] Version version)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(version != null, "version");

            Name = name;
            Version = version;
        }
    }
}
