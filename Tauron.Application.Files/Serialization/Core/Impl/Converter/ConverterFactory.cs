using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal static class ConverterFactory
    {
        [NotNull]
        public static SimpleConverter<string> CreateConverter([CanBeNull] MemberInfo member, [CanBeNull] Type targetType)
        {
            if (member == null) throw new ArgumentNullException("member");
            if (targetType == null) throw new ArgumentNullException("targetType");

            if (targetType == typeof (string)) return new StringCnverter();
            if (targetType.BaseType == typeof (Enum)) return new GenericEnumConverter(targetType);

            return new TypeConverterConverter(GetConverter(member, targetType));
        }

        [NotNull]
        public static SimpleConverter<IEnumerable<string>> CreateListConverter([NotNull] MemberInfo member,
            [NotNull] Type targeType)
        {
            if (member == null) throw new ArgumentNullException("member");
            if (targeType == null) throw new ArgumentNullException("targeType");

            var builder = new ListBuilder(targeType);

            var converter = CreateConverter(member, builder.ElemenType);

            return new UniversalListConverter(converter, builder);
        }

        [NotNull]
        private static TypeConverter GetConverter([NotNull] MemberInfo info, [NotNull] Type memberType)
        {
            Type targetType = memberType;

            var attr = info.GetCustomAttributes<TypeConverterAttribute>().FirstOrDefault();
            if (attr == null) return TypeDescriptor.GetConverter(targetType);

            Type target = GetTypeFromName(attr.ConverterTypeName, targetType);
            if (target == null) return TypeDescriptor.GetConverter(targetType);

            var converter = Activator.CreateInstance(target) as TypeConverter;
            return converter ?? TypeDescriptor.GetConverter(targetType);
        }

        [CanBeNull]
        private static Type GetTypeFromName([NotNull] string typeName, [CanBeNull] Type memberType)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            int num = typeName.IndexOf(',');
            Type type = null;
            if (num == -1 && memberType != null) type = memberType.Assembly.GetType(typeName);
            if (type == null) type = Type.GetType(typeName);
            if (type == null && num != -1) type = Type.GetType(typeName.Substring(0, num));
            return type;
        }
    }
}
