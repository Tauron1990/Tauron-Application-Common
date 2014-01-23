namespace Tauron.Application.Files.Build.Conditions
{
    public sealed class SmallerThenOperator : ConditionOperator
    {
        protected override string OperatorValue
        {
            get
            {
                return "&lt";
            }
        }
    }
}