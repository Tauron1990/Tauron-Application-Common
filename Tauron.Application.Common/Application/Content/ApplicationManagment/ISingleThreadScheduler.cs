#region

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The SingleThreadScheduler interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (SingleThreadSchedulerContracts))]
    public interface ISingleThreadScheduler
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        bool IsBackground { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        void Queue([NotNull] Action task);

        #endregion
    }

    [ContractClassFor(typeof (ISingleThreadScheduler))]
    internal abstract class SingleThreadSchedulerContracts : ISingleThreadScheduler
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        public bool IsBackground { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        public void Queue(Action task)
        {
            Contract.Requires<ArgumentNullException>(task != null, "task");
        }

        #endregion
    }
}