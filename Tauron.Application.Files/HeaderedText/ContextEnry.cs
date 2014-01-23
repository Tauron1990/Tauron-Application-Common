using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class ContextEnry
    {
        public string Key { get; private set; }
        public string Content { get; private set; }

        internal ContextEnry(string key, string content)
        {
            Key = key;
            Content = content;
        }
    }
}
