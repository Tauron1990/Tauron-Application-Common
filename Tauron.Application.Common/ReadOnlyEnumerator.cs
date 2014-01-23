// The file ReadOnlyEnumerator.cs is part of Tauron.Application.Common.
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
// <copyright file="ReadOnlyEnumerator.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The read only enumerator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>
    ///     The read only enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [PublicAPI]
    public class ReadOnlyEnumerator<T> : IEnumerable<T>
    {
        #region Fields

        /// <summary>The _enumerable.</summary>
        private readonly IEnumerable<T> _enumerable;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyEnumerator{T}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ReadOnlyEnumerator{T}" /> Klasse.
        ///     Initializes a new instance of the <see cref="ReadOnlyEnumerator{T}" /> class.
        /// </summary>
        /// <param name="enumerable">
        ///     The enumerable.
        /// </param>
        public ReadOnlyEnumerator(IEnumerable<T> enumerable)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null, "enumerable");

            _enumerable = enumerable;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator<T>>() != null);

            return _enumerable.GetEnumerator();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator>() != null);

            return GetEnumerator();
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_enumerable != null);
        }

        #endregion
    }
}