using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class BraceExpression : ConditionElement
    {
        [CanBeNull]
        public ConditionElement Content { get; set; }

        public override string FormattedValue
        {
            get
            {
                return "(" + Content + ")";
            }
        }
    }
}