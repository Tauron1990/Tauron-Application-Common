using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlElementSerializer
    {
        private readonly XmlElementTarget _target;
        private readonly SimpleConverter<string> _converter;

        public XmlElementSerializer([NotNull] XmlElementTarget target, [NotNull] SimpleConverter<string> converter)
        {
            _target = target;
            _converter = converter;
        }

        [CanBeNull]
        public static IEnumerable<XElement> GetElements([NotNull] XElement ele, bool toWrite, [CanBeNull] XmlElementTarget target, int count)
        {
            XElement currentElement = ele;
            if (target == null) return new[] {currentElement};

            while (true)
            {
                if (target.XNamespace == null) target.XNamespace = XNamespace.None;

                if (target.TargetType == XmlElementTargetType.Attribute) throw new InvalidOperationException("Attributes Not Supported");

                XName currentName = target.XNamespace + target.Name;

                if (target.SubElement != null)
                {
                    XElement temp = currentElement.Element(currentName);

                    if (temp == null)
                    {
                        if (!toWrite) return null;
                        temp = new XElement(currentName);
                        currentElement.Add(temp);
                    }

                    currentElement = temp;
                    target = target.SubElement;
                    continue;
                }

                IEnumerable<XElement> els = currentElement.Elements(currentName).ToArray();

                if (!toWrite) return els;

                int realCount = els.Count();

                if (realCount == count) return els;

                var elements = new List<XElement>(els);

                for (int i = realCount; i < count; i++)
                {
                    var temp = new XElement(currentName);
                    currentElement.Add(temp);
                    elements.Add(temp);
                }

                return elements;
            }
        }

        [CanBeNull]
        public static XObject GetElement([NotNull] XElement ele, bool toWrite, [CanBeNull] XmlElementTarget target)
        {
            XElement currentElement = ele;

            if (target == null || (target.TargetType == XmlElementTargetType.Root && target.SubElement == null)) return currentElement;

            while (true)
            {
                if (target.XNamespace == null) target.XNamespace = XNamespace.None;

                XName currentName = target.XNamespace + target.Name;
                switch (target.TargetType)
                {
                    case XmlElementTargetType.Attribute:
                        XAttribute attr = currentElement.Attribute(currentName);
                        if (attr == null)
                        {
                            if (!toWrite) return null;
                            attr = new XAttribute(currentName, string.Empty);
                            currentElement.Add(attr);
                        }
                        return attr;
                    case XmlElementTargetType.Element:
                        XElement temp = currentElement.Element(currentName);
                        if (temp == null)
                        {
                            if (!toWrite) return null;
                            temp = new XElement(currentName);
                            currentElement.Add(temp);
                        }

                        currentElement = temp;

                        if (target.SubElement != null)
                        {
                            target = target.SubElement;
                            continue;
                        }
                        return currentElement;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [NotNull]
        public object Deserialize([NotNull] XElement file)
        {
            object obj = null;
            ProgressElement(SerializerMode.Deserialize,
                            GetElement(file, false, _target), ref obj);
            return obj;
        }

        public void Serialize([NotNull] object target, [NotNull] XElement file)
        {
            object obj = target;
            ProgressElement(SerializerMode.Serialize,
                            GetElement(file, true, _target), ref obj);
        }

        private void ProgressString(SerializerMode mode, [NotNull] ref string str, [CanBeNull] ref object obj)
        {
            switch (mode)
            {
                case SerializerMode.Deserialize:
                    obj = _converter.ConvertBack(str);
                    break;
                case SerializerMode.Serialize:
                    str = _converter.Convert(obj);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        private void ProgressElement(SerializerMode mode, [CanBeNull] XObject xobj, [NotNull] ref object obj)
        {
            if (xobj == null) return;

            var attr = xobj as XAttribute;

            string str = attr != null ? attr.Value : ((XElement) xobj).Value;

            switch (mode)
            {
                case SerializerMode.Deserialize:
                    ProgressString(mode, ref str, ref obj);
                    break;
                case SerializerMode.Serialize:
                    ProgressString(mode, ref str, ref obj);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }

            if (mode != SerializerMode.Serialize) return;

            if (attr != null) attr.Value = str;
            else ((XElement) xobj).Value = str;
        }

        [CanBeNull]
        public Exception VerifException()
        {
            if (_converter == null) return new ArgumentNullException("Converter");

            var e = _converter.VerifyError();
            if (e != null) return e;

            return _target == null ? new ArgumentNullException("Xml Tree") : null;
        }
    }
}