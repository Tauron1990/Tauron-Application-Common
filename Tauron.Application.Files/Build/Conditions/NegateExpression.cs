using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class NegateExpression : ConditionElement
    {
        [CanBeNull]
        public ConditionElement ToNegate { get; set; }

        public override string FormattedValue
        {
            get
            {
                return "!" + ToNegate;
            }
        }
    }
}