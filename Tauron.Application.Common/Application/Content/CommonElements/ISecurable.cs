
#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurable.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Defines the semantics of classes equipped with object-level security.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Security.Principal;

#endregion

namespace Tauron.Application
{
    /// <summary>Defines the semantics of classes equipped with object-level security.</summary>
    [ContractClass(typeof (SecurableContracts))]
    public interface ISecurable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether a given user is enrolled in at least one role.
        /// </summary>
        /// <param name="identity">
        ///     The user.
        /// </param>
        /// <param name="roles">
        ///     An comma-separated list of roles.
        /// </param>
        /// <returns>
        ///     <c>true</c> is the user is enrolled in at least one of the roles,
        ///     otherwise <c>false</c>.
        /// </returns>
        bool IsUserInRole(IIdentity identity, string roles);

        #endregion
    }

    [ContractClassFor(typeof (ISecurable))]
    internal abstract class SecurableContracts : ISecurable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The is user in role.
        /// </summary>
        /// <param name="identity">
        ///     The identity.
        /// </param>
        /// <param name="roles">
        ///     The roles.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsUserInRole(IIdentity identity, string roles)
        {
            Contract.Requires<ArgumentNullException>(identity != null, "identity");
            Contract.Requires<ArgumentNullException>(roles != null, "roles");

            return true;
        }

        #endregion
    }
}