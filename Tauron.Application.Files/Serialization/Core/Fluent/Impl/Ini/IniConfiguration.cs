using System;
using Tauron.Application.Files.Serialization.Core.Impl;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class IniConfiguration : SerializerRootConfigurationBase, IIniSerializerConfiguration
    {
        private readonly Type _targetType;
        private readonly ObjectBuilder _builder;
        private readonly SimpleMapper<IniContext> _mapper = new SimpleMapper<IniContext>();

        public IniConfiguration([NotNull] Type targetType)
        {
            _targetType = targetType;
            _builder = new ObjectBuilder(targetType);
        }

        public override ISerializer ApplyInternal()
        {
            var ser = new IniSerializer(_builder, _mapper);

            VerifyErrors(ser);

            return ser;
        }

        public IConstructorConfiguration<IIniSerializerConfiguration> ConfigConstructor()
        {
            return new ConstructorConfiguration<IIniSerializerConfiguration>(_builder, this);
        }

        public IIniSectionSerializerConfiguration FromSection(string name)
        {
            return new IniSectionConfiguration(name, _mapper, this, _targetType);
        }

        public ISerializerToMemberConfiguration<IIniSerializerConfiguration> MapSerializer<TToSerial>(string memberName)
        {
            return
                new SerializerToMemberConfiguration<IIniSerializerConfiguration, IniContext>(memberName, this, _mapper,
                                                                                             typeof (TToSerial));
        }
    }
}
