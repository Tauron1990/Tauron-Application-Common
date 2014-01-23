namespace Tauron.Application.Files.Build.Conditions
{
    public sealed class GreaterThenOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "&gt";
            }
        }
    }
}
