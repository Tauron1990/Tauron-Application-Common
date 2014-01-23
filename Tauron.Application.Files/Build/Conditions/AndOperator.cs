using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class AndOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "and";
            }
        }
    }
}
