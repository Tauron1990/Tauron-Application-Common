using Tauron.Application.Files.Ini;
using Tauron.Application.Files.Ini.Parser;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini
{
    internal class IniContext : ContextImplBase
    {
        [NotNull]
        public IniFile File { get; private set; }

        public IniContext([NotNull] SerializationContext context)
            : base(context)
        {
            File = context.SerializerMode == SerializerMode.Deserialize
                       ? IniFile.Parse(TextReader)
                       : new IniFile();
        }

        protected override void Dispose(bool disposing)
        {
            if (Original.SerializerMode != SerializerMode.Serialize)
            {
                base.Dispose(disposing);
                return;
            }

            new IniWriter(File, TextWriter).Write();

            base.Dispose(disposing);
        }
    }
}
