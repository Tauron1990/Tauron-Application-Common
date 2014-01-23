using System;
using Tauron.Application.Files.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Impl;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class HeaderedFileSerializerConfiguration : SerializerRootConfigurationBase, IHeaderedFileSerializerConfiguration
    {
        private readonly Type _targetType;
        private readonly ObjectBuilder _builder;
        private readonly SimpleMapper<HeaderdFileContext> _mapper = new SimpleMapper<HeaderdFileContext>();

        private FileDescription _description = new FileDescription();

        public HeaderedFileSerializerConfiguration([NotNull] Type targetType)
        {
            _targetType = targetType;
            _builder = new ObjectBuilder(targetType);
        }

        public override ISerializer ApplyInternal()
        {
            var serializer = new HeaderedTextSerializer(_builder, _mapper, _description);

            VerifyErrors(serializer);

            return serializer;
        }

        public IConstructorConfiguration<IHeaderedFileSerializerConfiguration> ConfigConstructor()
        {
            return new ConstructorConfiguration<IHeaderedFileSerializerConfiguration>(_builder, this);
        }

        public IHeaderedFileKeywordConfiguration AddKeyword(string key)
        {
            return new HeaderedFileKeyCofiguration(this, _mapper, key, MappingType.SingleKey, _targetType);
        }

        public IHeaderedFileKeywordConfiguration AddKeywordList(string key)
        {
            return new HeaderedFileKeyCofiguration(this, _mapper, key, MappingType.MultiKey, _targetType);
        }

        public IHeaderedFileKeywordConfiguration MapContent()
        {
            return new HeaderedFileKeyCofiguration(this, _mapper, "Content", MappingType.Content, _targetType);
        }
    }
}
