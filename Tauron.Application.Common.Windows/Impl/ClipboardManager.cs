using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using NLog;
using Tauron.Application.Ioc;
using Tauron.Application.Models.Interfaces;
using Tauron.Application.UIConnector;

namespace Tauron.Application.Common.Windows.Impl
{
    [Export(typeof(IClipboardManager))]
    public class ClipboardManager : IClipboardManager
    {
        private static ILogger _logger = LogManager.GetLogger(nameof(IClipboardManager));

        [Inject]
        public IUIConnector Connector { get; set; }

        public IClipboardViewer CreateViewer(IWindow target, bool registerForClose, bool performInitialization) => new ClipboardViewer(target, registerForClose, performInitialization);

        public bool ContainsText() => Clipboard.ContainsText();

        public string GetText() => Clipboard.GetText();

        public void SetValue(object value)
        {
            Exception ex = null;

            Connector.ApplicationConnector.Dispatcher.Invoke(() =>
            {
                try
                {
                    switch (value)
                    {
                        case string text:
                            Clipboard.SetText(text);
                            break;
                        case IDataObject dataObject:
                            Clipboard.SetDataObject(dataObject);
                            break;
                        case StringCollection files:
                            Clipboard.SetFileDropList(files);
                            break;
                        case byte[] audioBytes:
                            Clipboard.SetAudio(audioBytes);
                            break;
                        case Stream audioStream:
                            Clipboard.SetAudio(audioStream);
                            break;
                        case BitmapSource bitmap:
                            Clipboard.SetImage(bitmap);
                            break;
                        default:
                            Clipboard.SetDataObject(value);
                            break;
                    }

                    try
                    {
                        Clipboard.Flush();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                    }
                }
                catch (Exception e)
                {
                    ex = e;
                }
            });

            if (ex != null) throw ex;
        }
    }
}