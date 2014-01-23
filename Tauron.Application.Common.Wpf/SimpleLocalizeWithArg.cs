// The file SimpleLocalizeWithArg.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="SimpleLocalizeWithArg.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The simple localize with arg.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Markup;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The simple localize with arg.</summary>
    [MarkupExtensionReturnType(typeof (string))]
    public class SimpleLocalizeWithArg : SimpleLocalize
    {
        #region Public Properties

        /// <summary>Gets the arg.</summary>
        [CanBeNull]
        public string Arg { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The service provider.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var value = base.ProvideValue(serviceProvider) as string;

            if (value == null) return "null";

            if (value == string.Empty) return "null";

            try
            {
                return value.SFormat(Arg);
            }
            catch (FormatException)
            {
                return "null";
            }
        }

        #endregion
    }
}