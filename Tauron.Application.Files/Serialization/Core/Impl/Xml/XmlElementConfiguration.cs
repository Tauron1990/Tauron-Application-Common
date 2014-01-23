using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class XmlElementConfiguration : IXmlElementConfiguration
    {
        private readonly IXmlSerializerConfiguration _config;
        private readonly SimpleMapper<XmlElementContext> _mapper;
        private readonly XmlElementTarget _target;
        private readonly XmlElementTarget _root;
        private readonly Type _targetType;

        private string _member;
        private SimpleConverter<string> _converter;

        public XmlElementConfiguration([NotNull] IXmlSerializerConfiguration config,
                                       [NotNull] SimpleMapper<XmlElementContext> mapper,
                                       [NotNull] XmlElementTarget target, [NotNull] XmlElementTarget root,
                                       [NotNull] Type targetType)
        {
            _config = config;
            _mapper = mapper;
            _target = target;
            _root = root;
            _targetType = targetType;
        }

        public IXmlSerializerConfiguration Apply()
        {
            if (_member == null) _member = _target.Name;

            var map = new XmlMapper(_member, _targetType, _converter, _root);
            _mapper.Entries.Add(map);

            return _config;
        }

        public IXmlElementConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        public IXmlElementConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IXmlSerializerConfiguration WithSerializer(XmlSerializer serializer)
        {
            var map = new XmlSerializerMapper(_member, _targetType, serializer, _target);
            _mapper.Entries.Add(map);

            return _config;
        }

        public IXmlSerializerConfiguration WithSubSerializer<TTarget>(ISerializer serializer)
        {
            var map = new XmlSubSerializerMapper(_member, typeof (TTarget), serializer as ISubSerializer,
                _root);
            _mapper.Entries.Add(map);

            return _config;
        }

        public IXmlElementConfiguration Element(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Element, Name = name};
            _target.SubElement = target;

            return new XmlElementConfiguration(_config, _mapper, target, _root, _targetType);
        }

        public IXmlElementConfiguration WithNamespace(XNamespace xNamespace)
        {
            _target.XNamespace = xNamespace;

            return this;
        }

        public IXmlAttributConfiguration Attribute(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Attribute, Name = name};
            _target.SubElement = target;
            return new XmlAttributeConfiguration(_config, _root, target, _mapper, _targetType);
        }

        public IXmlListElementConfiguration WithElements(string name)
        {
            var target = new XmlElementTarget { TargetType = XmlElementTargetType.Element, Name = name };

            return new XmlListElementConfiguration(_config, _mapper, _target, _root, target, _targetType);
        }
    }
}