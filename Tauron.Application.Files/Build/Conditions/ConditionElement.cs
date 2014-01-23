using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    public abstract class ConditionElement
    {
        [NotNull]
        public abstract string FormattedValue { get; }

        public override string ToString()
        {
            return FormattedValue;
        }
    }
}