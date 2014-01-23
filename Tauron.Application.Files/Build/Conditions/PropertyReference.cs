using System.Globalization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class PropertyReference : ConditionReference
    {
        public override string FormattedValue
        {
            get
            {
                return FomatPropertyWithQuotes(PropertyName);
            }
        }

        [NotNull]
        public string PropertyName { get; set; }

        [NotNull]
        internal static string FomatPropertyWithQuotes([NotNull] string name)
        {
            return string.Format(CultureInfo.InvariantCulture, "'{0}'", FomatProperty(name));
        }

        [NotNull]
        internal static string FomatProperty([NotNull] string name)
        {
            return string.Format(CultureInfo.InvariantCulture, "$({0})", name);
        }

    }
}
