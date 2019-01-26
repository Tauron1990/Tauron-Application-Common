using System.Windows;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.Windows.Impl
{
    [Export(typeof(IClipboardManager))]
    public class ClipboardManager : IClipboardManager
    {
        public IClipboardViewer CreateViewer(IWindow target, bool registerForClose, bool performInitialization) => new ClipboardViewer(target, registerForClose, performInitialization);

        public bool ContainsText() => Clipboard.ContainsText();

        public string GetText() => Clipboard.GetText();
    }
}