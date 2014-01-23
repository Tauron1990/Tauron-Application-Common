using System;
using System.ComponentModel;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class TypeConverterConverter : SimpleConverter<string>
    {
        private readonly TypeConverter _converter;

        public TypeConverterConverter([NotNull] TypeConverter converter)
        {
            if (converter == null) throw new ArgumentNullException("converter");

            _converter = converter;
        }

        public override object ConvertBack([NotNull] string target)
        {
            return _converter.ConvertFromString(target);
        }

        [NotNull]
        public override string Convert(object source)
        {
            return _converter.ConvertToString(source);
        }
    }
}
