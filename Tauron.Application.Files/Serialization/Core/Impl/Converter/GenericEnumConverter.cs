using System;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class GenericEnumConverter : SimpleConverter<string>
    {
        private readonly Type _enumType;

        public GenericEnumConverter([NotNull] Type enumType)
        {
            if (enumType.BaseType != typeof (Enum)) throw new SerializerElementException("The Type is no Enum");

            _enumType = enumType;
        }

        public override object ConvertBack([NotNull] string target)
        {
            return Enum.Parse(_enumType, target);
        }

        [NotNull]
        public override string Convert(object source)
        {
            if (source == null) return string.Empty;

            return source.ToString();
        }
    }
}
