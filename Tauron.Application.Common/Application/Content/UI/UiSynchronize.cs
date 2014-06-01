#region

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

// ReSharper disable once CheckNamespace
namespace Tauron.Application
{
    /// <summary>The UISynchronize interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (UISynchronizeContracts))]
    public interface IUISynchronize
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The begin invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [NotNull]
        Task BeginInvoke([NotNull] Action action);

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        void Invoke([NotNull] Action action);

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="TReturn" />.
        /// </returns>
        TReturn Invoke<TReturn>([NotNull] Func<TReturn> action);

        #endregion
    }

    [ContractClassFor(typeof (IUISynchronize))]
    internal abstract class UISynchronizeContracts : IUISynchronize
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The begin invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task BeginInvoke(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null, "action");
            Contract.Ensures(Contract.Result<Task>() != null);

            return null;
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        public void Invoke(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null, "action");
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TReturn" />.
        /// </returns>
        public TReturn Invoke<TReturn>(Func<TReturn> action)
        {
            Contract.Requires<ArgumentNullException>(action != null, "action");

            return default(TReturn);
        }

        #endregion
    }

    /// <summary>The ui synchronize.</summary>
    [PublicAPI]
    public static class UiSynchronize
    {
        #region Public Properties

        /// <summary>Gets or sets the synchronize.</summary>
        [NotNull]
        public static IUISynchronize Synchronize { get; set; }

        #endregion
    }
}