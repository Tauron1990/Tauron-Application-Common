using System;
using System.Reflection;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini
{
    internal sealed class SingleIniMapper : MappingEntryBase<IniContext>
    {
        private readonly SimpleConverter<string> _converter;
        private readonly string _section;
        private readonly string _key;

        public SingleIniMapper([CanBeNull] string membername, [NotNull] Type targetType, [CanBeNull] SimpleConverter<string> converter, [NotNull] string section, [NotNull] string key) 
            : base(membername, targetType)
        {
            _converter = converter;
            _section = GetSectionForIni(targetType, section);
            _key = GetKeyForIni(TargetMember, key);

            if (_converter == null && MemberType != null) _converter = ConverterFactory.CreateConverter(TargetMember, MemberType);
        }

        protected override void Deserialize(object target, IniContext context)
        {
            var value = _converter.ConvertBack(context.File[_section].GetData(_key).Value);
            SetValue(target, value);
        }

        protected override void Serialize(object target, IniContext context)
        {
            var value = GetValue(target);
            context.File.SetData(_section, _key, _converter.Convert(value));
        }

        public override Exception VerifyError()
        {
            var e = base.VerifyError();

            if(_converter == null)
                e = new ArgumentNullException("Converter");

            return e ?? _converter.VerifyError();
        }

        [NotNull]
        public static string GetKeyForIni([CanBeNull] MemberInfo member, [CanBeNull] string key)
        {
            if (key == null) return member == null ? string.Empty : member.Name;

            return key;
        }

        [NotNull]
        public static string GetSectionForIni([CanBeNull] Type target, [CanBeNull] string section)
        {
            if (section == null) return target == null ? string.Empty : target.Name;
            return section;
        }
    }
}
