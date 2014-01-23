using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class OrOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "or";
            }
        }
    }
}