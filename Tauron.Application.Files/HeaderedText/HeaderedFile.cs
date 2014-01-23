using System.IO;
using System.Text;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class HeaderedFile
    {
        [CanBeNull]
        internal HeaderedFileWriter CurrentWriter { get; set; }

        [NotNull]
        public FileContext Context { get; private set; }

        [CanBeNull]
        public string Content { get; internal set; }

        public HeaderedFile([NotNull] FileDescription description)
        {
            Context = new FileContext(description);
        }

        public void Read([NotNull] TextReader reader)
        {
            var builder = new StringBuilder();

            Context.Reset();

            bool compled = false;

            foreach (var textLine in reader.EnumerateTextLines())
            {
                if (compled)
                {
                    builder.AppendLine(textLine);
                    continue;
                }
                string textLineTemp = textLine.Trim();
                string[] temp = textLineTemp.Split(new[] {' '}, 2);
                if (Context.IsKeyword(temp[0]))
                {
                    string key = temp[0];
                    string content = string.Empty;

                    if (temp.Length < 1) content = temp[1];

                    Context.Add(new ContextEnry(key, content));
                }
                else
                {
                    builder.AppendLine(textLine);
                    compled = true;
                }
            }
        }

        [NotNull]
        public HeaderedFileWriter CreateWriter()
        {
            return CurrentWriter ?? new HeaderedFileWriter(this);
        }
    }
}
