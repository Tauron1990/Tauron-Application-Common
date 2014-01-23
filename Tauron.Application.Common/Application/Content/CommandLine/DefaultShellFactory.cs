// The file DefaultShellFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="DefaultShellFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The default shell factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The default shell factory.</summary>
    [PublicAPI]
    public class DefaultShellFactory : IShellFactory
    {
        #region Fields

        /// <summary>The _shell type.</summary>
        private readonly Type _shellType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultShellFactory" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="DefaultShellFactory" /> Klasse.
        ///     Initializes a new instance of the <see cref="DefaultShellFactory" /> class.
        /// </summary>
        /// <param name="shellType">
        ///     The shell type.
        /// </param>
        public DefaultShellFactory(Type shellType)
        {
            Contract.Requires<ArgumentNullException>(shellType != null, "shellType");

            _shellType = shellType;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object CreateView()
        {
            return Activator.CreateInstance(_shellType);
        }

        #endregion
    }
}