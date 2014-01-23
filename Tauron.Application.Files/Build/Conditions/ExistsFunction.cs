using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public sealed class ExistsFunction : FunctionCondition
    {
        protected override string FunctionName
        {
            get
            {
                return "Exists";
            }
        }
    }
}