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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The workspace manager.
    /// </summary>
    /// <typeparam name="TWorkspace">
    /// </typeparam>
    [PublicAPI]
    public sealed class WorkspaceManager<TWorkspace> : UISyncObservableCollection<TWorkspace>
        where TWorkspace : class, ITabWorkspace
    {
        #region Fields

        private readonly IWorkspaceHolder _holder;
        private ITabWorkspace _activeItem;

        #endregion

        [NotNull]
        public ITabWorkspace ActiveItem

        {
            get { return _activeItem; }
            set
            {
                _activeItem = value;

                if(Equals(_activeItem, value)) return;

                if(_activeItem != null) _activeItem.OnDeactivate();

                _activeItem = value;
                _activeItem.OnActivate();

                OnPropertyChanged(new PropertyChangedEventArgs("ActiveItem"));
            }
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkspaceManager{TWorkspace}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WorkspaceManager{TWorkspace}" /> Klasse.
        /// </summary>
        /// <param name="holder">
        ///     The holder.
        /// </param>
        public WorkspaceManager([NotNull] IWorkspaceHolder holder)
        {
            Contract.Requires<ArgumentNullException>(holder != null, "holder");

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
        public void AddRange([NotNull] IEnumerable<TWorkspace> items)
        {
            Contract.Requires<ArgumentNullException>(items != null, "items");

            foreach (TWorkspace item in items.Where(it => it != null)) Add(item);
        }

        #endregion

        #region Methods

        /// <summary>The clear items.</summary>
        protected override void ClearItems()
        {
            foreach (TWorkspace workspace in Items) UnRegisterWorkspace(workspace);

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
        protected override void InsertItem(int index, [CanBeNull] TWorkspace item)
        {
            if(item == null) return;

            if (index < Count) UnRegisterWorkspace(this[index]);

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
            UnRegisterWorkspace(this[index]);
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
        protected override void SetItem(int index, [CanBeNull] TWorkspace item)
        {
            if(item == null) return;

            UnRegisterWorkspace(this[index]);
            _holder.Register(item);
            base.SetItem(index, item);
        }

        #endregion

        private void UnRegisterWorkspace([NotNull] ITabWorkspace space)
        {
            space.OnClose();
            _holder.UnRegister(space);
        }
    }
}