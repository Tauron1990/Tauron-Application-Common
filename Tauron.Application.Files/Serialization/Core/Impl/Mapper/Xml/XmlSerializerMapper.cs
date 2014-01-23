using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal sealed class XmlSerializerMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly XmlSerializer _serializer;
        private readonly XmlElementTarget _xmlElementTarget;

        public XmlSerializerMapper([CanBeNull] string membername, [NotNull] Type targetType,
                                   [CanBeNull] XmlSerializer serializer, [CanBeNull] XmlElementTarget xmlElementTarget)
            : base(membername, targetType)
        {
            _serializer = serializer;
            _xmlElementTarget = xmlElementTarget;
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            XObject obj = XmlElementSerializer.GetElement(context.XElement, false, _xmlElementTarget);
            var ele = obj as XElement;
            if(ele == null)
                return;
            SetValue(target, _serializer.Deserialize(ele.CreateReader(ReaderOptions.OmitDuplicateNamespaces)));
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            XObject obj = XmlElementSerializer.GetElement(context.XElement, true, _xmlElementTarget);
            var ele = obj as XElement;
            if(ele == null)
                throw new InvalidOperationException("Attributes not Supported");
            _serializer.Serialize(context.XElement.CreateWriter(), GetValue(target));
        }

        public override Exception VerifyError()
        {
            Exception e = base.VerifyError();

            if(_serializer == null)
                e = new ArgumentNullException("Serializer");
            if(_xmlElementTarget == null)
                e = new ArgumentNullException("Xml Tree");

            XmlElementTarget target = _xmlElementTarget;

            while (target != null)
            {
                if (target.TargetType == XmlElementTargetType.Attribute)
                {
                    e = new SerializerElementException("Attributes Not Supported");
                    break;
                }

                target = target.SubElement;
            }

            return e;
        }
    }
}