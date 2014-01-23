using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public class SmallerThenEqualOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "&lt=";
            }
        }
    }
}