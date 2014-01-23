using System;
using System.Collections.Generic;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class IniKeyConfiguration : IIniKeySerializerConfiguration
    {
        private readonly string _section;
        private readonly IIniSerializerConfiguration _configuration;
        private readonly SimpleMapper<IniContext> _mapper;
        private readonly bool _isSingle;
        private readonly Type _targetType;

        private string _member;
        private string _key;
        private SimpleConverter<string> _converter;
        private SimpleConverter<IEnumerable<string>> _listConverter;

        public IniKeyConfiguration([NotNull] string section, [NotNull] IIniSerializerConfiguration configuration,
                                   [NotNull] SimpleMapper<IniContext> mapper, bool isSingle, [NotNull] Type targetType)
        {
            _section = section;
            _configuration = configuration;
            _mapper = mapper;
            _isSingle = isSingle;
            _targetType = targetType;
        }


        public IIniKeySerializerConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        public IIniKeySerializerConfiguration WithKey(string name)
        {
            _key = name;
            return this;
        }

        public IIniKeySerializerConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IIniKeySerializerConfiguration WithConverter(SimpleConverter<IEnumerable<string>> converter)
        {
            _listConverter = converter;
            return this;
        }

        public IIniSerializerConfiguration Apply()
        {
            MappingEntry<IniContext> entry;

            if(_isSingle)
                entry = new SingleIniMapper(_member, _targetType, _converter, _section, _key);
            else
                entry = new ListIniMapper(_member, _targetType, _listConverter, _section, _key);

            _mapper.Entries.Add(entry);
            return _configuration;
        }
    }
}
