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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The ui sync observable collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [DebuggerNonUserCode, PublicAPI]
    public class UISyncObservableCollection<TType> : ObservableCollection<TType>
    {
        private class DummySync : IUISynchronize
        {
            public Task BeginInvoke(Action action)
            {
                action();
                return null;
            }

            public void Invoke(Action action)
            {
                action();
            }

            public TReturn Invoke<TReturn>(Func<TReturn> action)
            {
                return action();
            }
        }

        private IUISynchronize _synchronize;

        public UISyncObservableCollection()
        {
        }

        public UISyncObservableCollection([NotNull] IEnumerable<TType> enumerable)
            : base(enumerable)
        {
        }

        [NotNull]
        private IUISynchronize InternalUISynchronize
        {
            get
            {
                if (_synchronize != null) return _synchronize;
                _synchronize = UiSynchronize.Synchronize ?? new DummySync();

                return _synchronize;
            }
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
            InternalUISynchronize.Invoke(() => base.OnCollectionChanged(e));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnPropertyChanged([NotNull] PropertyChangedEventArgs e)
        {
            InternalUISynchronize.Invoke(() => base.OnPropertyChanged(e));
        }

        #endregion
    }
}