// The file IWindow.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Diagnostics.Contracts;
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

        /// <summary>Gets the handle.</summary>
        /// <value>The handle.</value>
        IntPtr Handle { get; }

        /// <summary>Gets or sets the title.</summary>
        [NotNull]
        string Title { get; set; }

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

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        object TranslateForTechnology();

        #endregion
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

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object TranslateForTechnology()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return null;
        }

        #endregion
    }
}