using System;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal sealed class InternalXmlSerializer : SerializerBase<XmlElementContext>
    {
        private readonly string _rootName;
        private readonly XDeclaration _declaration;
        private readonly XNamespace _xNamespace;

        public InternalXmlSerializer([NotNull] ObjectBuilder builder, [NotNull] SimpleMapper<XmlElementContext> mapper,
                                     [NotNull] string rootName, [CanBeNull] XDeclaration declaration,
                                     [CanBeNull] XNamespace xNamespace)
            : base(builder, mapper, ContextMode.Text)
        {
            _rootName = rootName;
            _declaration = declaration;
            _xNamespace = xNamespace;
        }

        public override XmlElementContext BuildContext(SerializationContext context)
        {
            return new XmlElementContext(context, _declaration, _xNamespace, _rootName);
        }

        public override void CleanUp(XmlElementContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.Dispose();
        }
    }
}
