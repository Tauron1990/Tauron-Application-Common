// The file SingleThreadScheduler.cs is part of Tauron.Application.Common.
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
// <copyright file="SingleThreadScheduler.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The single thread scheduler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Threading;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The single thread scheduler.</summary>
    [PublicAPI]
    [Export(typeof (ISingleThreadScheduler))]
    [NotShared]
    public sealed class SingleThreadScheduler : ISingleThreadScheduler, IDisposable
    {
        #region Fields

        /// <summary>The _tasks.</summary>
        private readonly BlockingCollection<Action> _tasks = new BlockingCollection<Action>();

        /// <summary>The _thread.</summary>
        private readonly Thread _thread;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleThreadScheduler" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SingleThreadScheduler" /> Klasse.
        ///     Initializes a new instance of the <see cref="SingleThreadScheduler" /> class.
        /// </summary>
        public SingleThreadScheduler()
        {
            _thread = new Thread(EnterLoop);
            IsBackground = true;

            _thread.Start();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SingleThreadScheduler" /> class.
        ///     Finalisiert eine Instanz der <see cref="SingleThreadScheduler" /> Klasse.
        ///     Finalizes an instance of the <see cref="SingleThreadScheduler" /> class.
        /// </summary>
        ~SingleThreadScheduler()
        {
            Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        public bool IsBackground
        {
            get { return _thread.IsBackground; }

            set { _thread.IsBackground = value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _tasks.CompleteAdding();
            _tasks.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        public void Queue(Action task)
        {
            _tasks.Add(task);
        }

        #endregion

        #region Methods

        /// <summary>The enter loop.</summary>
        [ContractVerification(false)]
        private void EnterLoop()
        {
            foreach (Action item in _tasks.GetConsumingEnumerable()) item();
        }

        #endregion
    }
}