// The file UISyncObservableCollection.cs is part of Tauron.Application.Common.
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
// <copyright file="UISyncObservableCollection.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ui sync observable collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The ui sync observable collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    public class UISyncObservableCollection<TType> : ObservableCollection<TType>
    {
        public UISyncObservableCollection()
        {
        }

        public UISyncObservableCollection([NotNull] IEnumerable<TType> enumerable)
            : base(enumerable)
        {
        }

        #region Methods

        /// <summary>
        ///     The on collection changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            UiSynchronize.Synchronize.Invoke(() => base.OnCollectionChanged(e));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            UiSynchronize.Synchronize.Invoke(() => base.OnPropertyChanged(e));
        }

        #endregion
    }
}