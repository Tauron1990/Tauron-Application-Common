using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class StaticReference : ConditionReference
    {
        public override string FormattedValue
        {
            get
            {
                return string.Format("'{0}'", Value);
            }
        }

        [NotNull]
        public string Value { get; set; }
    }
}