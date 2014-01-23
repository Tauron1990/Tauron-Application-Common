using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class GreaterThenEqualOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "&gt=";
            }
        }
    }
}