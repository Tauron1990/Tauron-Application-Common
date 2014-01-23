using System;
using Tauron.Application.Files.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class HeaderedTextSerializer : SerializerBase<HeaderdFileContext>
    {
        private readonly FileDescription _description;

        public HeaderedTextSerializer([NotNull] ObjectBuilder builder, [NotNull] SimpleMapper<HeaderdFileContext> mapper,
            [NotNull] FileDescription description)
            : base(builder, mapper, ContextMode.Text)
        {
            _description = description;
        }

        public override HeaderdFileContext BuildContext(SerializationContext context)
        {
            return new HeaderdFileContext(context, _description);
        }

        public override void CleanUp(HeaderdFileContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.Dispose();
        }
    }
}
