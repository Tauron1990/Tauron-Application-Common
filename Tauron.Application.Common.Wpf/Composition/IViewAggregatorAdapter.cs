// The file IViewAggregatorAdapter.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="IViewAggregatorAdapter.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ViewAggregatorAdapter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The ViewAggregatorAdapter interface.</summary>
    public interface IViewAggregatorAdapter
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The adapt.
        /// </summary>
        /// <param name="dependencyObject">
        ///     The dependency object.
        /// </param>
        void Adapt(DependencyObject dependencyObject);

        /// <summary>
        ///     The add views.
        /// </summary>
        /// <param name="views">
        ///     The views.
        /// </param>
        void AddViews(IEnumerable<object> views);

        /// <summary>
        ///     The can adapt.
        /// </summary>
        /// <param name="dependencyObject">
        ///     The dependency object.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanAdapt(DependencyObject dependencyObject);

        /// <summary>The release.</summary>
        void Release();

        #endregion
    }
}