// The file LimitedList.cs is part of Tauron.Application.Common.
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

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitedList.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The limited list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The limited list.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [PublicAPI]
    public class LimitedList<T> : Collection<T>
    {
        #region Fields

        /// <summary>The _limit.</summary>
        private int _limit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LimitedList{T}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="LimitedList{T}" /> Klasse.
        ///     Initializes a new instance of the <see cref="LimitedList{T}" /> class.
        /// </summary>
        public LimitedList()
        {
            Limit = -1;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the limit.</summary>
        /// <value>The limit.</value>
        public int Limit
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() != 0);

                return _limit;
            }

            set
            {
                Contract.Requires<ArgumentException>(value != 0, "The Limit of the Collection cannot be set to 0.");

                _limit = value;
                CleanUp();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The insert item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            CleanUp();
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
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            CleanUp();
        }

        /// <summary>The clean up.</summary>
        private void CleanUp()
        {
            if (Limit == -1) return;

            while (Count > Limit) Items.RemoveAt(0);
        }

        #endregion
    }
}