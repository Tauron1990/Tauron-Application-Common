using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using JetBrains.Annotations;

namespace Tauron.Application
{
    public sealed class WpfWindow : IWindow, IDisposable
    {
        public event EventHandler Closed
        {
            add => _window.Closed += value;
            remove => _window.Closed -= value;
        }
        
        private readonly Window _window;

        private bool _disposed;

        private HwndSource _source;
        
        public WpfWindow([NotNull] Window window) => _window = Argument.NotNull(window, nameof(window));

        ~WpfWindow() => Dispose(false);

        public IntPtr Handle
        {
            get
            {
                EnsureSource();
                return _source.Handle;
            }
        }

        public bool HandleAndHookSupported => true;

        public bool? DialogResult
        {
            get { return UiSynchronize.Synchronize.Invoke(() => _window.DialogResult); }
            set { UiSynchronize.Synchronize.Invoke(() => _window.DialogResult = value); }
        }
        
        public string Title
        {
            get { return UiSynchronize.Synchronize.Invoke(() => _window.Title); }
            set { UiSynchronize.Synchronize.Invoke(() => _window.Title = value); }
        }
        
        public void Dispose() => Dispose(true);

        public bool? IsVisible
        {
            get
            {
                if (_disposed) return null;
                return _window.Dispatcher.Invoke(() => _window.IsVisible);
            }
        }
        
        public void AddHook(WindowHook winProc)
        {
            if (winProc == null) throw new ArgumentNullException(nameof(winProc));
            EnsureSource();
            UiSynchronize.Synchronize.Invoke(() => _source.AddHook(Create(winProc)));
        }
        
        public void Close()
        {
            UiSynchronize.Synchronize.Invoke(() =>
            {
                Dispose();
                _window.Close();
            });
        }
        
        public void RemoveHook(WindowHook winProc)
        {
            Argument.NotNull(winProc, nameof(winProc));
            EnsureSource();
            UiSynchronize.Synchronize.Invoke(() => _source.RemoveHook(Create(winProc)));
        }
        
        public void Show()
        {
            UiSynchronize.Synchronize.Invoke(() =>
            {
                var info = _window.DataContext as IShowInformation;

                info?.OnShow(this);
                _window.Show();
                info?.AfterShow(this);
            });
        }

        public Task ShowDialogAsync(IWindow window)
        {
            return UiSynchronize.Synchronize.BeginInvoke(() =>
            {
                _window.Owner = window?.TranslateForTechnology() as Window;

                var info = _window.DataContext as IShowInformation;

                info?.OnShow(this);
                _window.ShowDialog();
                info?.AfterShow(this);
            });
        }
        
        public object TranslateForTechnology() => _window;

        public void Focus() => UiSynchronize.Synchronize.BeginInvoke(() => _window.Focus());

        public void Hide() => UiSynchronize.Synchronize.Invoke(() => _window.Hide());

        public object Result
        {
            get
            {
                return UiSynchronize.Synchronize.Invoke(() =>
                {
                    var temp = _window.DataContext as IResultProvider;
                    return temp?.Result;
                });
            }
        }
        private void CheckDiposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());
        }

        [NotNull]
        private HwndSourceHook Create([NotNull] WindowHook hook) => (HwndSourceHook) Delegate.CreateDelegate(typeof(HwndSourceHook), hook.Target, hook.Method);

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

            _source?.Dispose();
            _disposed = true;
        }

        private void EnsureSource()
        {
            CheckDiposed();
            if (_source == null)
                _source = HwndSource.FromHwnd(new WindowInteropHelper(_window).Handle);
        }
    }
}