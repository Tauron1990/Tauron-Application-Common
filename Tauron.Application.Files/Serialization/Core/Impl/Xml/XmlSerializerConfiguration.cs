using System;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Fluent.Impl;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class XmlSerializerConfiguration : SerializerRootConfigurationBase, IXmlSerializerConfiguration
    {
        private readonly string _rootName;
        private readonly XDeclaration _declaration;
        private readonly XNamespace _xNamespace;
        private readonly Type _targetType;
        private readonly SimpleMapper<XmlElementContext> _mapper = new SimpleMapper<XmlElementContext>();
        private readonly ObjectBuilder _builder;

        public XmlSerializerConfiguration([NotNull] string rootName, [CanBeNull] XDeclaration declaration,
                                          [CanBeNull] XNamespace xNamespace, [NotNull] Type targetType)
        {
            _rootName = rootName;
            _declaration = declaration;
            _xNamespace = xNamespace;
            _targetType = targetType;
            _builder = new ObjectBuilder(_targetType);
        }

        public override ISerializer ApplyInternal()
        {
            var temp = new InternalXmlSerializer(_builder, _mapper, _rootName, _declaration, _xNamespace);
            var errors = temp.Errors;
            if (errors != null)
                throw errors;

            return temp;
        }

        public IConstructorConfiguration<IXmlSerializerConfiguration> ConfigConstructor()
        {
            return new ConstructorConfiguration<IXmlSerializerConfiguration>(_builder, this);
        }

        public IXmlAttributConfiguration WithAttribut(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Attribute, Name = name};
            return new XmlAttributeConfiguration(this, target, target, _mapper, _targetType);
        }

        public IXmlElementConfiguration WithElement(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Element, Name = name};
            return new XmlElementConfiguration(this, _mapper, target, target, _targetType);
        }

        public IXmlListElementConfiguration WithElements(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Element, Name = name};
            var empty = new XmlElementTarget {TargetType = XmlElementTargetType.Root};

            return new XmlListElementConfiguration(this, _mapper, empty, empty, target, _targetType);
        }

        public IXmlSerializerConfiguration WithSubSerializer<TTarget>(ISerializer serializer, string member)
        {
            var map = new XmlSubSerializerMapper(member, typeof (TTarget),
                serializer as ISubSerializer,
                new XmlElementTarget {TargetType = XmlElementTargetType.Root});
            _mapper.Entries.Add(map);

            return this;
        }
    }
}