// The file UserTask.cs is part of Tauron.Application.Common.
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
// <copyright file="UserTask.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The user task.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The user task.</summary>
    public class UserTask : ITask
    {
        #region Fields

        /// <summary>The _callback.</summary>
        private readonly Action _callback;

        private readonly TaskCompletionSource<object> _task;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserTask" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="UserTask" /> Klasse.
        ///     Initializes a new instance of the <see cref="UserTask" /> class.
        /// </summary>
        /// <param name="callback">
        ///     The callback.
        /// </param>
        /// <param name="sync">
        ///     The sync.
        /// </param>
        public UserTask([NotNull] Action callback, bool sync)
        {
            Contract.Requires<ArgumentNullException>(callback != null, "callback");

            _callback = callback;
            Synchronize = sync;
            _task = new TaskCompletionSource<object>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether synchronize.</summary>
        /// <value>The synchronize.</value>
        public bool Synchronize { get; private set; }

        /// <summary>Gets the task.</summary>
        [NotNull]
        public Task Task
        {
            get { return _task.Task; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute.</summary>
        public void Execute()
        {
            try
            {
                _callback();
                _task.SetResult(null);
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }

        #endregion
    }
}