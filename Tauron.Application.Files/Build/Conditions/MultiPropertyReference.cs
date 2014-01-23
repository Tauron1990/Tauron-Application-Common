using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build.Conditions
{
    [PublicAPI]
    public class MultiPropertyReference : ConditionReference
    {
        public bool WithQuotes { get; set; }

        public MultiPropertyReference(bool withQuotes, [CanBeNull] params string[] propertys)
        {
            WithQuotes = withQuotes;
            Propertys = propertys == null ? new List<string>() : new List<string>(propertys);
        }

        public override string FormattedValue
        {
            get
            {
                if (Propertys.Count == 1 && !WithQuotes)
                    return PropertyReference.FomatPropertyWithQuotes(Propertys[0]);
                
                var builder = new StringBuilder();

                if (WithQuotes)
                    builder.Append("'");

                builder.Append(PropertyReference.FomatProperty(Propertys[0]));

                foreach (var property in Propertys.Skip(1))
                {
                    builder.Append("|").Append(PropertyReference.FomatProperty(property));
                }

                if (WithQuotes)
                    builder.Append("'");

                return builder.ToString();
            }
        }

        [NotNull]
        public List<string> Propertys { get; private set; }
    }
}