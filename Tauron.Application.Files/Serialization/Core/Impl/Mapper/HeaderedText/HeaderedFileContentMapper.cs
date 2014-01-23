using System;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderedFileContentMapper : MappingEntryBase<HeaderdFileContext>
    {
        private readonly SimpleConverter<string> _converter;

        public HeaderedFileContentMapper([CanBeNull] string membername, [NotNull] Type targetType, [NotNull] SimpleConverter<string> converter) 
            : base(membername, targetType)
        {
            _converter = converter;

// ReSharper disable once AssignNullToNotNullAttribute
            if (_converter == null && TargetMember != null) _converter = ConverterFactory.CreateConverter(TargetMember, MemberType);
        }

        protected override void Deserialize(object target, HeaderdFileContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            SetValue(target, _converter.ConvertBack(context.Content));
        }

        protected override void Serialize(object target, HeaderdFileContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.CurrentWriter.Content = _converter.Convert(GetValue(target));
        }
    }
}