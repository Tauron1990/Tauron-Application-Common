using System;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace Tauron.Application
{
    public delegate IntPtr WindowHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

    [PublicAPI]
    public interface IWindow
    {
        [CanBeNull]
        object Result { get; }
        
        event EventHandler Closed;
        
        void Focus();

        void Hide();
        
        [NotNull]
        string Title { get; set; }

        IntPtr Handle { get; }

        bool HandleAndHookSupported { get; }

        bool? DialogResult { set; get; }

        bool? IsVisible { get; }
        
        void AddHook([NotNull] WindowHook winProc);
        
        void Close();
        
        void RemoveHook([NotNull] WindowHook winProc);
        
        void Show();

        [NotNull]
        Task ShowDialogAsync([CanBeNull] IWindow window);

        [NotNull]
        object TranslateForTechnology();
        
    }
}