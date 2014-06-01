#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITauronEnviroment.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The TauronEnviroment interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The TauronEnviroment interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (TauronEnviromentContracts))]
    public interface ITauronEnviroment
    {
        #region Public Properties

        /// <summary>Gets or sets the default profile path.</summary>
        /// <value>The default profile path.</value>
        [NotNull]
        string DefaultProfilePath { get; set; }

        /// <summary>Gets the local application data.</summary>
        /// <value>The local application data.</value>
        [NotNull]
        string LocalApplicationData { get; }

        /// <summary>Gets the local application temp folder.</summary>
        /// <value>The local application temp folder.</value>
        [NotNull]
        string LocalApplicationTempFolder { get; }

        /// <summary>Gets the local download folder.</summary>
        /// <value>The local download folder.</value>
        [NotNull]
        string LocalDownloadFolder { get; }

        #endregion

        #region Public Methods and Operators

        [NotNull]
        IEnumerable<string> GetProfiles([NotNull] string application);

        /// <summary>
        ///     The search for folder.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        string SearchForFolder(Guid id);

        #endregion
    }

    [ContractClassFor(typeof (ITauronEnviroment))]
    internal abstract class TauronEnviromentContracts : ITauronEnviroment
    {
        #region Public Properties

        /// <summary>Gets or sets the default profile path.</summary>
        /// <value>The default profile path.</value>
        public string DefaultProfilePath
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }

            set { Contract.Requires<ArgumentNullException>(value != null, "value"); }
        }

        /// <summary>Gets the local application data.</summary>
        /// <value>The local application data.</value>
        public string LocalApplicationData
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }
        }

        /// <summary>Gets the local application temp folder.</summary>
        /// <value>The local application temp folder.</value>
        public string LocalApplicationTempFolder
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }
        }

        /// <summary>Gets the local download folder.</summary>
        /// <value>The local download folder.</value>
        public string LocalDownloadFolder
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get profiles.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<string> GetProfiles(string application)
        {
            Contract.Requires<ArgumentNullException>(application != null, "application");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
            return null;
        }

        /// <summary>
        ///     The search for folder.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string SearchForFolder(Guid id)
        {
            Contract.Ensures(Contract.Result<string>() != null);

            return null;
        }

        #endregion
    }
}