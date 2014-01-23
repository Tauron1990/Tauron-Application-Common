using System;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class XmlAttributeConfiguration : IXmlAttributConfiguration
    {
        private readonly IXmlSerializerConfiguration _config;
        private readonly XmlElementTarget _rootTarget;
        private readonly XmlElementTarget _target;
        private readonly SimpleMapper<XmlElementContext> _mapper;
        private readonly Type _targetType;

        private string _member;
        private SimpleConverter<string> _converter;

        public XmlAttributeConfiguration([NotNull] IXmlSerializerConfiguration config,
                                         [NotNull] XmlElementTarget rootTarget, [NotNull] XmlElementTarget target,
                                         [NotNull] SimpleMapper<XmlElementContext> mapper, [NotNull] Type targetType)
        {
            _config = config;
            _rootTarget = rootTarget;
            _target = target;
            _mapper = mapper;
            _targetType = targetType;
        }

        public IXmlSerializerConfiguration Apply()
        {
            if (_member == null && _target != null) _member = _target.Name;

            var map = new XmlMapper(_member, _targetType, _converter, _rootTarget);
            _mapper.Entries.Add(map);

            return _config;
        }

        public IXmlAttributConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        public IXmlAttributConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IXmlAttributConfiguration WithNamespace(XNamespace xNamespace)
        {
            _target.XNamespace = xNamespace;
            return this;
        }
    }
}