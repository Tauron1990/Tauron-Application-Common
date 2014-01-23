using System.Globalization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public abstract class FunctionCondition : ConditionElement
    {
        [CanBeNull]
        public ConditionElement Argument { get; set; }

        [NotNull]
        protected abstract string FunctionName { get; }

        public override string FormattedValue
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}({1})", FunctionName, Argument);
            }
        }
    }
}