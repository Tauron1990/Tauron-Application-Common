﻿using System;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class XmlListElementConfiguration : IXmlListElementConfiguration, IXmlListAttributeConfiguration
    {
        #region Element

        private readonly IXmlSerializerConfiguration _config;
        private readonly SimpleMapper<XmlElementContext> _mapper;
        private readonly XmlElementTarget _target;
        private readonly XmlElementTarget _root;
        private readonly XmlElementTarget _parentTarget;
        private readonly Type _targeType;

        private string _member;
        private SimpleConverter<string> _converter;

        public XmlListElementConfiguration([NotNull] IXmlSerializerConfiguration config,
                                           [NotNull] SimpleMapper<XmlElementContext> mapper,
                                           [NotNull] XmlElementTarget target, [NotNull] XmlElementTarget root,
                                           [NotNull] XmlElementTarget parentTarget, [NotNull] Type targeType)
        {
            _config = config;
            _mapper = mapper;
            _target = target;
            _root = root;
            _parentTarget = parentTarget;
            _targeType = targeType;
        }

        public IXmlSerializerConfiguration Apply()
        {
            if (_member == null) _member = _target.Name;

            var map = new XmlListMapper(_member, _targeType, _parentTarget, _root, _converter);
            _mapper.Entries.Add(map);

            return _config;
        }

        IXmlListAttributeConfiguration IXmlRootConfiguration<IXmlListAttributeConfiguration>.WithNamespace(XNamespace xNamespace)
        {
            WithNamespace(xNamespace);
            return this;
        }

        public IXmlListElementConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        IXmlListAttributeConfiguration IWithMember<IXmlListAttributeConfiguration>.WithConverter(SimpleConverter<string> converter)
        {
            WithConverter(converter);
            return this;
        }

        IXmlListAttributeConfiguration IWithMember<IXmlListAttributeConfiguration>.WithMember(string name)
        {
            WithMember(name);
            return this;
        }

        public IXmlListElementConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IXmlListElementConfiguration Element(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Element, Name = name};
            _target.SubElement = target;

            return new XmlListElementConfiguration(_config, _mapper, target, _root, _parentTarget, _targeType);
        }

        public IXmlListElementConfiguration WithNamespace(XNamespace xNamespace)
        {
            _target.XNamespace = xNamespace;
            return this;
        }

        public IXmlSerializerConfiguration WithSubSerializer<TSerisalize>(ISerializer serializer)
        {
            if (_member == null) _member = _target.Name;

            var mapper = new XmlListSubSerializerMapper(_member, typeof (TSerisalize), _parentTarget, _root,
                serializer as ISubSerializer);
            _mapper.Entries.Add(mapper);

            return _config;
        }

        public IXmlListAttributeConfiguration Attribute(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Attribute, Name = name};
            _target.SubElement = target;

            return new XmlListElementConfiguration(_config, _mapper, target, _root, _parentTarget, _targeType);
        }

        #endregion

    }
}