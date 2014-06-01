#region

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The CommandLineCommand interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (CommandLineCommandContracts))]
    public interface ICommandLineCommand
    {
        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        [NotNull]
        string CommandName { get; }

        /// <summary>Gets the factory.</summary>
        /// <value>The factory.</value>
        [CanBeNull]
        IShellFactory Factory { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        void Execute([NotNull] string[] args, [NotNull] IContainer container);

        #endregion
    }

    [ContractClassFor(typeof (ICommandLineCommand))]
    internal abstract class CommandLineCommandContracts : ICommandLineCommand
    {
        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        public string CommandName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return null;
            }
        }

        /// <summary>Gets the factory.</summary>
        /// <value>The factory.</value>
        public IShellFactory Factory { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        public void Execute(string[] args, IContainer container)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Requires<ArgumentNullException>(container != null, "container");
        }

        #endregion
    }
}