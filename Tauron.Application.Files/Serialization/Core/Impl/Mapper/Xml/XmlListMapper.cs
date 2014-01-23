using System;
using System.Linq;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal sealed class XmlListMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly XmlElementTarget _rootTarget;
        private readonly XmlElementSerializer _serializer;

        private readonly ListBuilder _listBuilder;

        public XmlListMapper([CanBeNull] string membername, [NotNull] Type targetType,
                             [NotNull] XmlElementTarget rootTarget, [NotNull] XmlElementTarget target,
                             [CanBeNull] SimpleConverter<string> converter)
            : base(membername, targetType)
        {
            _rootTarget = rootTarget;

            if (MemberType != null) _listBuilder = new ListBuilder(MemberType);

            if (converter == null && _listBuilder != null) converter = ConverterFactory.CreateConverter(TargetMember, _listBuilder.ElemenType);

// ReSharper disable once AssignNullToNotNullAttribute
            _serializer = new XmlElementSerializer(target, converter);
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            _listBuilder.Begin(null, false);

            XElement[] targetElements =
                XmlElementSerializer.GetElements(context.XElement, false, _rootTarget, -1).ToArray();

            if(targetElements == null)
                return;

            foreach (var targetElement in targetElements)
            {
                _listBuilder.Add(_serializer.Deserialize(targetElement));
            }

            SetValue(target, _listBuilder.End());
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            _listBuilder.Begin(GetValue(target), true);

            object[] content = _listBuilder.Objects;
            XElement[] targetElements =
                XmlElementSerializer.GetElements(context.XElement, true, _rootTarget, content.Length).ToArray();

            for (int i = 0; i < content.Length; i++)
            {
                _serializer.Serialize(content[i], targetElements[i]);
            }

            _listBuilder.End();
        }

        public override Exception VerifyError()
        {
            Exception e = base.VerifyError() ?? _serializer.VerifException() ?? _listBuilder.VerifyError();

            if(_rootTarget == null)
                e = new ArgumentNullException("Path to Elements: null");

            return e;
        }
    }
}