using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class NotEqualOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "!=";
            }
        }
    }
}