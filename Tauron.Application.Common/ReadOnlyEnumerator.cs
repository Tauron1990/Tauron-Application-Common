#region

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

        public ReadOnlyEnumerator([NotNull] IEnumerable<T> enumerable)
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