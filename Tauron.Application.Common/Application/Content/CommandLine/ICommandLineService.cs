#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The CommandLineService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The CommandLineService interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (CommandLineServiceContracts))]
    public interface ICommandLineService
    {
        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        [NotNull]
        IEnumerable<ICommandLineCommand> Commands { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        void Add([NotNull] ICommandLineCommand command);

        #endregion
    }

    [ContractClassFor(typeof (ICommandLineService))]
    internal abstract class CommandLineServiceContracts : ICommandLineService
    {
        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        public IEnumerable<ICommandLineCommand> Commands
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ICommandLineCommand>>() != null);

                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        public void Add(ICommandLineCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "command");
        }

        #endregion
    }
}