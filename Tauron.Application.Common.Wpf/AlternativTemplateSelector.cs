// The file AlternativTemplateSelector.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="AlternativTemplateSelector.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The alternativ template selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Tauron.Application
{
    /// <summary>The alternativ template selector.</summary>
    public sealed class AlternativTemplateSelector : DataTemplateSelector
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The select template.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <returns>
        ///     The <see cref="DataTemplate" />.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (item == null || container == null) return null;

            // ReSharper restore HeuristicUnreachableCode
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            string key = item.GetType().Name;
            var ele = container.As<FrameworkElement>();
            if (ele == null) return null;

            var temp = ele.TryFindResource(key).As<DataTemplate>();
            return temp;
        }

        #endregion
    }
}