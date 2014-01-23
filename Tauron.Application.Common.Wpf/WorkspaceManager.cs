// The file WorkspaceManager.cs is part of Tauron.Application.Common.Wpf.
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
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The WorkspaceHolder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Tauron.Application.Commands;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The WorkspaceHolder interface.</summary>
    public interface IWorkspaceHolder
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="workspace">
        ///     The workspace.
        /// </param>
        void Register([NotNull] ITabWorkspace workspace);

        /// <summary>
        ///     The un register.
        /// </summary>
        /// <param name="workspace">
        ///     The workspace.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un")]
        void UnRegister([NotNull] ITabWorkspace workspace);

        #endregion
    }

    /// <summary>The tab workspace.</summary>
    public abstract class TabWorkspace : ObservableObject, ITabWorkspace
    {
        #region Constants

        /// <summary>The close event name.</summary>
        protected const string CloseEventName = "CloseEvent";

        #endregion

        #region Fields

        private bool _canClose;

        private string _tile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabWorkspace" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TabWorkspace" /> Klasse.
        /// </summary>
        /// <param name="title">
        ///     The title.
        /// </param>
        protected TabWorkspace([NotNull] string title)
        {
            _tile = title;
            _canClose = true;
            CloseWorkspace = new SimpleCommand(obj => CanClose, obj => InvokeClose());
        }

        #endregion

        #region Public Events

        /// <summary>The close.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ITabWorkspace> Close
        {
            add { AddEvent(CloseEventName, value); }

            remove { RemoveEvent(CloseEventName, value); }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether can close.</summary>
        public bool CanClose
        {
            get { return _canClose; }

            set
            {
                _canClose = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets the close workspace.</summary>
        public ICommand CloseWorkspace { get; private set; }

        /// <summary>Gets or sets the title.</summary>
        public string Title
        {
            get { return _tile; }

            set
            {
                _tile = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The invoke close.</summary>
        public virtual void InvokeClose()
        {
            InvokeEvent(CloseEventName, this);
        }

        #endregion
    }

    /// <summary>
    ///     The workspace manager.
    /// </summary>
    /// <typeparam name="TWorkspace">
    /// </typeparam>
    [PublicAPI]
    public sealed class WorkspaceManager<TWorkspace> : ObservableCollection<TWorkspace>
        where TWorkspace : ITabWorkspace
    {
        #region Fields

        private readonly IWorkspaceHolder _holder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkspaceManager{TWorkspace}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WorkspaceManager{TWorkspace}" /> Klasse.
        /// </summary>
        /// <param name="holder">
        ///     The holder.
        /// </param>
        public WorkspaceManager(IWorkspaceHolder holder)
        {
            _holder = holder;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add range.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        public void AddRange(IEnumerable<TWorkspace> items)
        {
            foreach (TWorkspace item in items) Add(item);
        }

        #endregion

        #region Methods

        /// <summary>The clear items.</summary>
        protected override void ClearItems()
        {
            foreach (TWorkspace workspace in Items) _holder.UnRegister(workspace);

            base.ClearItems();
        }

        /// <summary>
        ///     The insert item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void InsertItem(int index, TWorkspace item)
        {
            if (index < Count) _holder.UnRegister(this[index]);

            _holder.Register(item);

            base.InsertItem(index, item);
        }

        /// <summary>
        ///     The remove item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        protected override void RemoveItem(int index)
        {
            _holder.UnRegister(this[index]);
            base.RemoveItem(index);
        }

        /// <summary>
        ///     The set item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void SetItem(int index, TWorkspace item)
        {
            _holder.UnRegister(this[index]);
            _holder.Register(item);
            base.SetItem(index, item);
        }

        #endregion
    }
}