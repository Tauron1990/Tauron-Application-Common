using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface IClipboardManager
    {
        [NotNull]
        IClipboardViewer CreateViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization);
    }
}