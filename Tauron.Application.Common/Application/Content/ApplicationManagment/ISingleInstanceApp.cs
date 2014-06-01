#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISingleInstanceApp.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The SingleInstanceApp interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The SingleInstanceApp interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (SingleInstanceAppContracts))]
    public interface ISingleInstanceApp
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The signal external command line args.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool SignalExternalCommandLineArgs([NotNull] IList<string> args);

        #endregion
    }

    [ContractClassFor(typeof (ISingleInstanceApp))]
    internal abstract class SingleInstanceAppContracts : ISingleInstanceApp
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The signal external command line args.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            return false;
        }

        #endregion
    }
}