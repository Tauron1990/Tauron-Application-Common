using System;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly XmlElementSerializer _serializer;

        public XmlMapper([CanBeNull] string membername, [NotNull] Type targetType,
                         [CanBeNull] SimpleConverter<string> converter, [NotNull] XmlElementTarget target)
            : base(membername, targetType)
        {
            if (converter == null && TargetMember != null) converter = ConverterFactory.CreateConverter(TargetMember, MemberType);

// ReSharper disable once AssignNullToNotNullAttribute
            _serializer = new XmlElementSerializer(target, converter);
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            SetValue(target, _serializer.Deserialize(context.XElement));
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            _serializer.Serialize(GetValue(target), context.XElement);
        }

        public override Exception VerifyError()
        {
            return base.VerifyError() ?? _serializer.VerifException();
        }
    }
}