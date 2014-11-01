#region

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The window hook.</summary>
    /// <param name="hwnd">The hwnd.</param>
    /// <param name="msg">The msg.</param>
    /// <param name="wParam">The w param.</param>
    /// <param name="lParam">The l param.</param>
    /// <param name="handled">The handled.</param>
    public delegate IntPtr WindowHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

    /// <summary>The Window interface.</summary>
    [ContractClass(typeof (WindowContracts))]
    [PublicAPI]
    public interface IWindow
    {
        #region Public Events

        /// <summary>The closed.</summary>
        event EventHandler Closed;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the title.</summary>
        [NotNull]
        string Title { get; set; }

        IntPtr Handle { get; }

        bool? DialogResult { set; get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        void AddHook([NotNull] WindowHook winProc);

        /// <summary>The close.</summary>
        void Close();

        /// <summary>
        ///     The remove hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        void RemoveHook([NotNull] WindowHook winProc);

        /// <summary>The show.</summary>
        void Show();

        [NotNull]
        Task ShowDialog([CanBeNull] IWindow window);

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        object TranslateForTechnology();

        #endregion

        void Focus();

        [CanBeNull]
        object Result { get; }
    }

    [ContractClassFor(typeof (IWindow))]
    internal abstract class WindowContracts : IWindow
    {
        #region Public Events

        /// <summary>The closed.</summary>
        public event EventHandler Closed;

        #endregion

        #region Public Properties

        /// <summary>Gets the handle.</summary>
        /// <value>The handle.</value>
        public IntPtr Handle
        {
            get
            {
                Contract.Ensures(Contract.Result<IntPtr>() != IntPtr.Zero);
                return IntPtr.Zero;
            }
        }

        public bool? DialogResult { get; set; }

        /// <summary>Gets or sets the title.</summary>
        public string Title
        {
            get { return null; }

            set { }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        public void AddHook(WindowHook winProc)
        {
            Contract.Requires<ArgumentNullException>(winProc != null, "winProc");
        }

        /// <summary>The close.</summary>
        public void Close()
        {
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
        }

        /// <summary>The show.</summary>
        public void Show()
        {
        }

        public Task ShowDialog(IWindow window)
        {
            
            return null;
        }

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object TranslateForTechnology()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return null;
        }

        public void Focus()
        {
        
        }

        public object Result { get; private set; }

        #endregion
    }
}