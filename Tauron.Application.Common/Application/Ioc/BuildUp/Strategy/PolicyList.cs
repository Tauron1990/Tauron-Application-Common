// The file PolicyList.cs is part of Tauron.Application.Common.
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
// <copyright file="PolicyList.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The policy list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The policy list.</summary>
    [PublicAPI]
    public class PolicyList
    {
        #region Fields

        /// <summary>The _list.</summary>
        private readonly GroupDictionary<Type, IPolicy> _list = new GroupDictionary<Type, IPolicy>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="policy">
        ///     The policy.
        /// </param>
        /// <typeparam name="TPolicy">
        /// </typeparam>
        public void Add<TPolicy>(TPolicy policy) where TPolicy : IPolicy
        {
            Contract.Requires<ArgumentNullException>(policy != null, "policy");

            _list.Add(typeof (TPolicy), policy);
        }

        /// <summary>The get.</summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns>
        ///     The <see cref="TPolicy" />.
        /// </returns>
        public TPolicy Get<TPolicy>()
        {
            ICollection<IPolicy> policies;
            if (_list.TryGetValue(typeof (TPolicy), out policies))
            {
                Contract.Assume(policies != null);
                return (TPolicy) policies.Single();
            }

            return default(TPolicy);
        }

        /// <summary>The get all.</summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<TPolicy> GetAll<TPolicy>()
        {
            Contract.Ensures(Contract.Result<IEnumerable<TPolicy>>() != null);

            ICollection<IPolicy> policies;
            if (!_list.TryGetValue(typeof (TPolicy), out policies)) return Enumerable.Empty<TPolicy>();

            Contract.Assume(policies != null);

            return policies.Cast<TPolicy>();
        }

        /// <summary>The remove.</summary>
        /// <typeparam name="TPolicy"></typeparam>
        public void Remove<TPolicy>()
        {
            _list.Remove(typeof (TPolicy));
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="policy">
        ///     The policy.
        /// </param>
        /// <typeparam name="TPolicy">
        /// </typeparam>
        public void Remove<TPolicy>(TPolicy policy) where TPolicy : IPolicy
        {
            _list.RemoveValue(policy);
        }

        #endregion
    }
}