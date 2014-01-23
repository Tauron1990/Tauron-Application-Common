using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    public abstract class ConditionOperator : ConditionElement
    {
        [CanBeNull]
        public ConditionElement Left { get; set; }

        [CanBeNull]
        public ConditionElement Right { get; set; }

        [NotNull]
        protected abstract string OperatorValue { get; }

        public override string FormattedValue
        {
            get
            {
                return Left + OperatorValue + Right;
            }
        }
    }
}
