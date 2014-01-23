using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Fluent.Impl;
using Tauron.Application.Files.Serialization.Core.Impl;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files
{
    [PublicAPI]
    public static class SerializerFactory
    {
        [NotNull]
        public static IBinaryConfiguration CreateBinary()
        {
            return new BinarySerializerConfiguration();
        }

        [NotNull]
        public static IIniSerializerConfiguration CreateIni<TType>()
        {
            return new IniConfiguration(typeof(TType));
        }

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>([NotNull] string rootName, [CanBeNull] XDeclaration xDeclaration, [NotNull] XNamespace rootNamespace)
        {
            return new XmlSerializerConfiguration(rootName, xDeclaration, rootNamespace, typeof(TType));
        }

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>([NotNull] string rootName, [CanBeNull] XDeclaration xDeclaration)
        {
            return CreateXml<TType>(rootName, xDeclaration, XNamespace.None);
        }

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>([NotNull] string rootName)
        {
            return CreateXml<TType>(rootName, null);
        }

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>()
        {
            return CreateXml<TType>("Root");
        }

        [NotNull]
        public static IHeaderedFileSerializerConfiguration CreateHeaderedFile<TType>()
        {
            return new HeaderedFileSerializerConfiguration(typeof(TType));
        }
    }
}
