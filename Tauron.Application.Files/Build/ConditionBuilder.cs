using Tauron.Application.Files.Build.Conditions;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public class ConditionBuilder
    {
        [NotNull]
        public ConditionElement Condition { get; set; }

        public override string ToString()
        {
            return Condition.ToString();
        }
    }
}
