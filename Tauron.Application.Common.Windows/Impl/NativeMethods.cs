using System;
using System.Runtime.InteropServices;

namespace Tauron.Application.Common.Windows.Impl
{
    internal static class NativeMethods
    {
        /// <summary>
        ///     Sent when the contents of the clipboard have changed.
        /// </summary>
        public const int WM_CLIPBOARDUPDATE = 0x031D;

        /// <summary>
        ///     Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        ///     Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("shell32.dll")]
        internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}