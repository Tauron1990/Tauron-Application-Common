#region

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The wpf window.</summary>
    public sealed class WpfWindow : IWindow, IDisposable
    {
        #region Fields

        private readonly Window _window;

        private bool _disposed;

        private HwndSource _source;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfWindow" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WpfWindow" /> Klasse.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        public WpfWindow([NotNull] Window window)
        {
            Contract.Requires<ArgumentNullException>(window != null, "window");

            _window = window;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="WpfWindow" /> class.
        ///     Finalisiert eine Instanz der <see cref="WpfWindow" /> Klasse.
        /// </summary>
        ~WpfWindow()
        {
            Dispose(false);
        }

        #endregion

        #region Public Events

        /// <summary>The closed.</summary>
        public event EventHandler Closed
        {
            add { _window.Closed += value; }

            remove { _window.Closed -= value; }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the handle.</summary>
        public IntPtr Handle
        {
            get
            {
                EnsureSource();
                return _source.Handle;
            }
        }

        /// <summary>Gets or sets the title.</summary>
        public string Title
        {
            get { return UiSynchronize.Synchronize.Invoke(() => _window.Title); }

            set { UiSynchronize.Synchronize.Invoke(() => _window.Title = value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     The add hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        public void AddHook(WindowHook winProc)
        {
            Contract.Requires<ArgumentNullException>(winProc != null, "winProc");

            EnsureSource();
            UiSynchronize.Synchronize.Invoke(() => _source.AddHook(Create(winProc)));
        }

        /// <summary>The close.</summary>
        public void Close()
        {
            UiSynchronize.Synchronize.Invoke(_window.Close);
        }

        /// <summary>
        ///     The remove hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        public void RemoveHook(WindowHook winProc)
        {
            Contract.Requires<ArgumentNullException>(winProc != null, "winProc");

            EnsureSource();
            UiSynchronize.Synchronize.Invoke(() => _source.RemoveHook(Create(winProc)));
        }

        /// <summary>The show.</summary>
        public void Show()
        {
            UiSynchronize.Synchronize.Invoke(_window.Show);
        }

        public Task ShowDialog(IWindow window)
        {
            return UiSynchronize.Synchronize.BeginInvoke(() =>
            {
                _window.Owner = window == null ? null : window.TranslateForTechnology() as Window;
                _window.ShowDialog();
            });
        }

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object TranslateForTechnology()
        {
            return _window;
        }

        public void Focus()
        {
            UiSynchronize.Synchronize.BeginInvoke(() => _window.Focus());
        }

        #endregion

        #region Methods

        private void CheckDiposed()
        {
            if (_disposed) throw new ObjectDisposedException(ToString());
        }

        [NotNull]
        private HwndSourceHook Create([NotNull] WindowHook hook)
        {
            return (HwndSourceHook) Delegate.CreateDelegate(typeof (HwndSourceHook), hook.Target, hook.Method);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) GC.SuppressFinalize(this);

            if (_source != null) _source.Dispose();

            _disposed = true;
        }

        private void EnsureSource()
        {
            CheckDiposed();
            if (_source == null) _source = HwndSource.FromHwnd(new WindowInteropHelper(_window).Handle);
        }

        #endregion
    }
}