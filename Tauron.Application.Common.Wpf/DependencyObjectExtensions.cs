// The file DependencyObjectExtensions.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="DependencyObjectExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The dependency object extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The dependency object extensions.</summary>
    [PublicAPI]
    public static class DependencyObjectExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The find resource.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public static object FindResource([NotNull] this DependencyObject obj, [NotNull] object key)
        {
            var temp1 = obj as FrameworkElement;
            var temp2 = obj as FrameworkContentElement;
            object result = null;
            if (temp1 != null) result = temp1.TryFindResource(key);

            if (result == null && temp2 != null) result = temp2.TryFindResource(key);

            return result;
        }

        /// <summary>
        ///     The find resource.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        [CanBeNull]
        public static TType FindResource<TType>([NotNull] this DependencyObject obj, [NotNull] object key) where TType : class
        {
            return FindResource(obj, key) as TType;
        }

        #endregion
    }
}