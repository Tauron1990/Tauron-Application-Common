using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class StringCnverter : SimpleConverter<string>
    {
        public override object ConvertBack([NotNull] string target)
        {
            return target;
        }

        [NotNull]
        public override string Convert(object source)
        {
            return source.ToString();
        }
    }
}
