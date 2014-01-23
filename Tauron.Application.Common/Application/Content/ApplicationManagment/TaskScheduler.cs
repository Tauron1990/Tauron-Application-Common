// The file TaskScheduler.cs is part of Tauron.Application.Common.
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
// <copyright file="TaskScheduler.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The task scheduler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The task scheduler.</summary>
    [PublicAPI]
    public sealed class TaskScheduler : IDisposable
    {
        #region Fields

        /// <summary>The _collection.</summary>
        private readonly BlockingCollection<ITask> _collection;

        /// <summary>The _synchronization context.</summary>
        private readonly IUISynchronize _synchronizationContext;

        /// <summary>The _disposed.</summary>
        [ContractPublicPropertyName("Disposed")] private bool _disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TaskScheduler" /> Klasse.
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        /// </summary>
        /// <param name="synchronizationContext">
        ///     The synchronization context.
        /// </param>
        public TaskScheduler([NotNull] IUISynchronize synchronizationContext)
        {
            Contract.Requires<ArgumentNullException>(synchronizationContext != null, "synchronizationContext");

            _synchronizationContext = synchronizationContext;
            _collection = new BlockingCollection<ITask>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TaskScheduler" /> Klasse.
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        /// </summary>
        public TaskScheduler()
        {
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="TaskScheduler" /> class.
        ///     Finalisiert eine Instanz der <see cref="TaskScheduler" /> Klasse.
        ///     Finalizes an instance of the <see cref="TaskScheduler" /> class.
        /// </summary>
        ~TaskScheduler()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary></summary>
        /// <value>The disposed.</value>
        public bool Disposed
        {
            get { return _disposed; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     The queue task.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [NotNull]
        public Task QueueTask([NotNull] ITask task)
        {
            Contract.Requires<ArgumentNullException>(task != null, "task");

            CheckDispose();
            if (task.Synchronize && _synchronizationContext != null)
            {
                return _synchronizationContext.BeginInvoke(task.Execute);
            }

            if (_collection == null)
            {
                CommonConstants.LogCommon(false, "Task Scheduler: Scheduler Not Initialized");
                task.Execute();
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }

            _collection.Add(task);
            return task.Task;
        }

        #endregion

        #region Methods

        /// <summary>The enter loop.</summary>
        [ContractVerification(false)]
        internal void EnterLoop()
        {
            foreach (ITask task in _collection.GetConsumingEnumerable()) task.Execute();

            _collection.Dispose();
        }

        /// <summary>The check dispose.</summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void CheckDispose()
        {
            if (_disposed) throw new ObjectDisposedException("TaskScheduler");
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="disposing">
        ///     The disposing.
        /// </param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        // ReSharper disable UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            // ReSharper restore UnusedParameter.Local
            if (_disposed) return;

            _disposed = true;
            if (_collection == null) return;

            _collection.CompleteAdding();
        }

        #endregion
    }
}