using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace Tauron.Application.Common.Windows.Impl
{
    [PublicAPI]
    public sealed class ClipboardViewer : IClipboardViewer
    {
        private bool _disposed;
        
        private ViewerSafeHandle _hWndViewer;
        
        private bool _isViewing;
        
        [CanBeNull]
        private IWindow _target;
        
        public event EventHandler ClipboardChanged;

        private class ViewerSafeHandle : SafeHandleMinusOneIsInvalid
        {
            public ViewerSafeHandle([NotNull] IWindow current)
                : base(true)
            {
                if (!NativeMethods.AddClipboardFormatListener(current.Handle))
                    throw new Win32Exception();

                SetHandle(current.Handle);
            }

            protected override bool ReleaseHandle() => NativeMethods.RemoveClipboardFormatListener(DangerousGetHandle());
        }
        public ClipboardViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            if (registerForClose) _target.Closed += TargetClosed;

            if (performInitialization) Initialize();
        }

        ~ClipboardViewer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_disposed) return;

            CloseCbViewer();
            if (_target != null) _target.Closed -= TargetClosed;
            _target = null;

            ClipboardChanged = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (_isViewing) return;

            if (_target != null)
            {
                _target.AddHook(WinProc); // start processing window messages
                _hWndViewer = new ViewerSafeHandle(_target ?? throw new InvalidOperationException("No Window")); // set this window as a viewer
            }

            _isViewing = true;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CloseCbViewer()
        {
            if (!_isViewing) return;
            _hWndViewer.Dispose();

            _hWndViewer = null;
            _target?.RemoveHook(WinProc);
            _isViewing = false;
        }
        
        private void OnClipboardChanged()
        {
            var handler = ClipboardChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }


        private void TargetClosed([NotNull] object sender, [NotNull] EventArgs e) => Dispose();

        [DebuggerStepThrough]
        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_CLIPBOARDUPDATE:
                    OnClipboardChanged();
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }
    }
}